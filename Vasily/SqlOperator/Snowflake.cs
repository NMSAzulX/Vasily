using System.Threading;

namespace System
{
    public class Snowflake<T>
    {
        //基准时间
        const long BaseTime = 1288834974657L;
        //可自定义长度
        const int CustomerSpacesLength = 22;

        static int _datacenter_length;
        static int _datanodes_length;
        static int _squence_mask;

        static long _info_sequence;
        static long _info_last_timestamp;
        static long _info_center_id;
        static long _info_node_id;

        readonly static object _lock = new object();
        static Snowflake()
        {
            _datacenter_length = 5;
        }
        /// <summary>
        /// 区域以及节点信息
        /// </summary>
        /// <param name="center_id">数据中心ID</param>
        /// <param name="node_id">节点ID</param>
        public static void SetNodesInfo(int center_id, int node_id)
        {
            _info_center_id = center_id;
            _info_node_id = node_id;
        }
        /// <summary>
        /// 比特位填充位设置
        /// </summary>
        /// <param name="data_center">数据中心的比特位，默认为5位</param>
        /// <param name="data_nodes">数据节点的比特位，默认为5位</param>
        public static void SetNodesLength(int data_center, int data_nodes)
        {
            int sequence_length = CustomerSpacesLength - data_center - data_nodes;

            if (data_nodes <= 0 || data_center <= 0 || sequence_length <= 0)
            {
                throw new ArgumentException("数据中心及节点位数设置有误，请检查！");
            }
            _squence_mask = ~(-1 << sequence_length);
            _datacenter_length = data_center;
            _datanodes_length = data_nodes;
        }

        /// <summary>
        /// 获取唯一ID
        /// </summary>
        public static long NextId { get { return GetId(); } }
        public static long GetId()
        {
            lock (_lock)
            {
                long timestamp = DateTime.Now.MillisecondsStamp();

                //如果上次生成时间和当前时间相同,在同一毫秒内
                if (_info_last_timestamp == timestamp)
                {
                    //sequence自增，和sequenceMask相与一下，去掉高位
                    _info_sequence = (_info_sequence + 1) & _squence_mask;
                    //判断是否溢出,也就是每毫秒内超过1024，当为1024时，与sequenceMask相与，sequence就等于0
                    if (_info_sequence == 0)
                    {
                        //等待到下一毫秒
                        timestamp = TilNextMillis();
                    }
                }
                else
                {
                    //如果和上次生成时间不同,重置sequence，就是下一毫秒开始，sequence计数重新从0开始累加,
                    //为了保证尾数随机性更大一些,最后一位可以设置一个随机数
                    _info_sequence = 0;
                }
                _info_last_timestamp = timestamp;
                return ((timestamp - BaseTime) << 22) | (_info_center_id << _datacenter_length) | (_info_node_id << _datanodes_length) | _info_sequence;
            }
        }

        // 防止产生的时间比之前的时间还要小（由于NTP回拨等问题）.
        protected static long TilNextMillis()
        {
            var timestamp = DateTime.Now.MillisecondsStamp();
            while (timestamp <= _info_last_timestamp)
            {
                Thread.Sleep(0);
                timestamp = DateTime.Now.MillisecondsStamp();

            }
            return timestamp;
        }
    }
}
