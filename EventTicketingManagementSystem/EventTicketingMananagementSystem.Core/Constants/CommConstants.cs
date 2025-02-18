namespace EventTicketingMananagementSystem.Core.Constants
{
    public class CommConstants
    {
        /// SEAT
        public static readonly string CST_SEAT_STATUS_DEFAULT = "available";

        public static readonly string CST_SEAT_STATUS_BOOKED = "Booked";

        public static readonly string CST_SEAT_STATUS_WAITING = "Waiting";

        public static readonly string CST_SEAT_STATUS_RESERVED = "Reserved";

        public static readonly string CST_SEAT_STATUS_CANCEL = "Cancel";

        public static readonly int CST_SEAT_NUM_START = 1;

        public static readonly int CST_SEAT_NUM_END = 16;

        public static readonly char CST_SEAT_ROW_START = 'A';

        public static readonly char CST_SEAT_ROW_END = 'J';

        public static readonly string CST_SEAT_TYPE_VIP = "vip";

        public static readonly string CST_SEAT_TYPE_NOR = "normal";
        ///

        ///PAYMENT
        public static readonly string CST_PAY_STATUS_PAID = "Paid";

        public static readonly string CST_PAY_STATUS_UNPAID = "Unpaid";

        public static readonly string CST_PAY_STATUS_PENDING = "Pending payment";

        public static readonly string CST_PAY_STATUS_CANCEL = "Cancel payment";

        ///

        ///S3 constants

        public static readonly string S3_BUCKET_NAME = "event-ticketing-management-system";

        ///
    }
}
