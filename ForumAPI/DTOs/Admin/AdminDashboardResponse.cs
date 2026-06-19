namespace ForumAPI.DTOs.Admin;

public class AdminDashboardResponse
{
    public int TongNguoiDung { get; set; }
    public int TongCauHoi { get; set; }
    public int TongCauTraLoi { get; set; }
    public int TongBinhLuan { get; set; }
    public int CauHoiDaXoa { get; set; }
    public int CauTraLoiDaXoa { get; set; }
    public int BinhLuanDaXoa { get; set; }
    public int ToCaoChoXuLy { get; set; }
    public IEnumerable<AdminChartItemResponse> NoiDungTheoLoai { get; set; } = Enumerable.Empty<AdminChartItemResponse>();
    public IEnumerable<AdminChartItemResponse> TrangThaiNoiDung { get; set; } = Enumerable.Empty<AdminChartItemResponse>();
    public IEnumerable<AdminChartItemResponse> TopTags { get; set; } = Enumerable.Empty<AdminChartItemResponse>();
    public IEnumerable<AdminChartItemResponse> CauHoiTheoChuyenMuc { get; set; } = Enumerable.Empty<AdminChartItemResponse>();
}

public class AdminChartItemResponse
{
    public string Ten { get; set; } = string.Empty;
    public int GiaTri { get; set; }
}
