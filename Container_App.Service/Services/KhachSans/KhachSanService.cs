using CloudinaryDotNet.Actions;
using Container_App.Common.Shared;
using Container_App.Core.Interface.KhachSans;
using Container_App.Core.Model.KhachSans;
using Container_App.Data.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.KhachSans
{
    public class KhachSanService : IKhachSanService
    {
        private readonly IStoredProcedureExecutor _executor;
        public KhachSanService(IStoredProcedureExecutor executor)
        {
            _executor = executor;
        }

        public async Task<IEnumerable<KhachSan>> FilterHotels(string? keyword,
            int? provinceCode,
            int? soKhach,
            DateTime? ngayNhanPhong,
            DateTime? ngayTraPhong,
            int startRow,
            int endRow)
        {
            try
            {
                var arr = new SqlParameter[]
                {
                    new SqlParameter("@Keyword",   SqlDbType.NVarChar, 255) { Value = (object?)keyword   ?? DBNull.Value },
                    new SqlParameter("@ProvinceCode",  SqlDbType.Int) { Value = (object?)provinceCode  ?? DBNull.Value },
                    new SqlParameter("@SoKhach",      SqlDbType.Int)         { Value = (object?)soKhach      ?? DBNull.Value },
                    new SqlParameter("@NgayNhanPhong",    SqlDbType.Date)         { Value = (object?)ngayNhanPhong    ?? DBNull.Value },
                    new SqlParameter("@NgayTraPhong",    SqlDbType.Date)         { Value = (object?)ngayTraPhong    ?? DBNull.Value },
                    new SqlParameter("@StartRow",  SqlDbType.Int)           { Value = startRow },
                    new SqlParameter("@EndRow",    SqlDbType.Int)           { Value = endRow }
                };
                return await _executor.QueryAsync<KhachSan>("sp_FilterHotel", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when FilterHotels: {ex.Message}");

                return Enumerable.Empty<KhachSan>();
            }
        }

        public async Task<IEnumerable<KhachSan>> LayDanhSachKhachSanAdmin(string keyword, string thanhPho, double viDo, double kinhDo, int soSao, string trangThai, int startRow, int endRow)
        {
            try
            {
                var arr = new SqlParameter[]
                {
                    new SqlParameter("@Keyword",   SqlDbType.NVarChar, 255) { Value = (object?)keyword   ?? DBNull.Value },
                    new SqlParameter("@ThanhPho",  SqlDbType.NVarChar, 255) { Value = (object?)thanhPho  ?? DBNull.Value },
                    new SqlParameter("@ViDo",      SqlDbType.Float)         { Value = (object?)viDo      ?? DBNull.Value },
                    new SqlParameter("@KinhDo",    SqlDbType.Float)         { Value = (object?)kinhDo    ?? DBNull.Value },
                    new SqlParameter("@SoSao",     SqlDbType.Int)           { Value = soSao },
                    new SqlParameter("@TrangThai", SqlDbType.NVarChar, 255) { Value = (object?)trangThai ?? DBNull.Value },
                    new SqlParameter("@StartRow",  SqlDbType.Int)           { Value = startRow },
                    new SqlParameter("@EndRow",    SqlDbType.Int)           { Value = endRow }
                };
                return await _executor.QueryAsync<KhachSan>("sp_LayDanhSachKhachSanAdmin", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get list KhachSan: {ex.Message}");

                return Enumerable.Empty<KhachSan>();
            }
        }

        public async Task<IEnumerable<KhachSan>> LayDanhSachKhachSanOwner(string keyword, string thanhPho, double viDo, double kinhDo, int soSao, string trangThai, Guid ownerId, int startRow, int endRow)
        {
            try
            {
                var arr = new SqlParameter[]
                {
                    new SqlParameter("@Keyword",   SqlDbType.NVarChar, 255) { Value = (object?)keyword   ?? DBNull.Value },
                    new SqlParameter("@ThanhPho",  SqlDbType.NVarChar, 255) { Value = (object?)thanhPho  ?? DBNull.Value },
                    new SqlParameter("@ViDo",      SqlDbType.Float)         { Value = (object?)viDo      ?? DBNull.Value },
                    new SqlParameter("@KinhDo",    SqlDbType.Float)         { Value = (object?)kinhDo    ?? DBNull.Value },
                    new SqlParameter("@SoSao",     SqlDbType.Int)           { Value = soSao },
                    new SqlParameter("@TrangThai", SqlDbType.NVarChar, 255) { Value = (object?)trangThai ?? DBNull.Value },
                    new SqlParameter("@UserId",   SqlDbType.UniqueIdentifier) { Value = (object?)ownerId ?? DBNull.Value },
                    new SqlParameter("@StartRow",  SqlDbType.Int)           { Value = startRow },
                    new SqlParameter("@EndRow",    SqlDbType.Int)           { Value = endRow }
                };
                return await _executor.QueryAsync<KhachSan>("sp_LayDanhSachKhachSanOwner", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get list KhachSan: {ex.Message}");

                return Enumerable.Empty<KhachSan>();
            }
        }

        public async Task<int> TaoKhachSan(KhachSan ks)
        {
            try
            {
                var tb = new DataTable();
                tb.Columns.Add("Url", typeof(string));
                foreach (var item in ks.Urls)
                {
                    tb.Rows.Add(item);
                }
                var arr = new[]
                {
                    new SqlParameter("@NguoiTao", ks.NguoiTao),
                    new SqlParameter("@TenKhachSan", ks.TenKhachSan),
                    new SqlParameter("@MoTa", ks.MoTa),
                    new SqlParameter("@DiaChi", ks.DiaChi),
                    new SqlParameter("@ThanhPho", ks.ThanhPho),
                    new SqlParameter("@ViDo", ks.ViDo),
                    new SqlParameter("@KinhDo", ks.KinhDo),
                    new SqlParameter("@SoSao", ks.SoSao),
                    new SqlParameter("@GioNhanPhong", ks.GioNhanPhong),
                    new SqlParameter("@GioTraPhong", ks.GioTraPhong),
                    new SqlParameter("@TrangThai", ks.TrangThai),
                    new SqlParameter("@ListImage", SqlDbType.Structured)
                    {
                        TypeName = "hotel_images",
                        Value = tb
                    }
                };
                return await _executor.ExecuteAsync("sp_TaoKhachSan", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when create KhachSan.");

                return -1;
            }
        }
    }
}
