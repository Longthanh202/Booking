using Booking.Common.Shared.Enum.Booking;

namespace Booking.Common.Shared.Func
{
    public static class BookingStatusName
    {
        public static string GetStatusName(TrangThaiDatPhong status)
        {
            return status switch
            {
                TrangThaiDatPhong.CHO_XAC_NHAN => "Chờ xác nhận",
                TrangThaiDatPhong.DA_XAC_NHAN => "Đã xác nhận",
                TrangThaiDatPhong.DA_CHECK_IN => "Đã nhận phòng",
                TrangThaiDatPhong.DA_CHECK_OUT => "Đã trả phòng",
                TrangThaiDatPhong.DA_HUY => "Đã hủy",
                TrangThaiDatPhong.TU_CHOI => "Từ chối",
                TrangThaiDatPhong.KHONG_DEN => "Không đến",
                _ => status.ToString()
            };
        }
    }
}
