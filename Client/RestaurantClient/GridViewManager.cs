using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

public class GridViewManager<T>
{
    private readonly DataGridView _gridView;
    private readonly Func<Task<List<T>>> _defaultLoadFunc;
    private readonly Func<T, object> _mapToDisplayModel;
    private readonly string _idPropertyName;

    // Cache danh sách nguồn để tránh load lại không cần thiết
    private List<T> _cachedSourceList = new List<T>();

    public GridViewManager(
        DataGridView gridView,
        Func<Task<List<T>>> loadDataFunc,
        Func<T, object> mapToDisplayModel,
        string idPropertyName = null)
    {
        _gridView = gridView;
        _defaultLoadFunc = loadDataFunc;
        _mapToDisplayModel = mapToDisplayModel;
        _idPropertyName = idPropertyName ?? DetectIdPropertyName();

        ConfigureGridView();
    }
    private void ConfigureGridView()
    {
        _gridView.AutoGenerateColumns = true;
        _gridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _gridView.MultiSelect = false;
        _gridView.ReadOnly = true;
        _gridView.AllowUserToAddRows = false;
        _gridView.AllowUserToDeleteRows = false;
        _gridView.RowHeadersVisible = false;
        _gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    }
    public async Task<bool> LoadDataAsync()
    {
        return await LoadDataAsync(_defaultLoadFunc);
    }
    public async Task<bool> LoadDataAsync(Func<Task<List<T>>> customLoadFunc)
    {
        try
        {
            _gridView.Enabled = false;
            _gridView.Cursor = Cursors.WaitCursor;

            var loadFunc = customLoadFunc ?? _defaultLoadFunc;
            var data = await loadFunc();

            // Cache lại danh sách nguồn
            _cachedSourceList = data ?? new List<T>();

            // Map sang display model
            var displayData = _cachedSourceList.Select(_mapToDisplayModel).ToList();

            _gridView.DataSource = displayData;

            // Clear selection sau khi load
            _gridView.ClearSelection();

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        finally
        {
            _gridView.Enabled = true;
            _gridView.Cursor = Cursors.Default;
        }
    }
    public T GetSelectedItem()
    {
        if (_gridView.SelectedRows.Count == 0)
            return default(T);

        try
        {
            // Lấy ID từ cột được hiển thị
            var displayIdColumn = GetDisplayIdColumnName();
            var selectedId = Convert.ToInt32(_gridView.SelectedRows[0].Cells[displayIdColumn].Value);

            // Tìm trong cached list
            return _cachedSourceList.FirstOrDefault(item => GetItemId(item) == selectedId);
        }
        catch
        {
            return default(T);
        }
    }
    public List<T> GetSelectedItems()
    {
        var selectedItems = new List<T>();

        if (_gridView.SelectedRows.Count == 0)
            return selectedItems;

        try
        {
            var displayIdColumn = GetDisplayIdColumnName();

            foreach (DataGridViewRow row in _gridView.SelectedRows)
            {
                var selectedId = Convert.ToInt32(row.Cells[displayIdColumn].Value);
                var item = _cachedSourceList.FirstOrDefault(i => GetItemId(i) == selectedId);
                if (item != null)
                    selectedItems.Add(item);
            }
        }
        catch { }

        return selectedItems;
    }
    public List<T> GetCachedData()
    {
        return _cachedSourceList;
    }
    public void ClearSelection()
    {
        _gridView.ClearSelection();
    }
    public void SelectById(int id)
    {
        var displayIdColumn = GetDisplayIdColumnName();

        foreach (DataGridViewRow row in _gridView.Rows)
        {
            if (Convert.ToInt32(row.Cells[displayIdColumn].Value) == id)
            {
                row.Selected = true;
                _gridView.FirstDisplayedScrollingRowIndex = row.Index;
                break;
            }
        }
    }
    private string DetectIdPropertyName()
    {
        var props = typeof(T).GetProperties();

        // Ưu tiên các tên phổ biến
        var commonIdNames = new[] { "MaNguoiDung", "MaMon", "MaBan", "MaHoaDon", "Id" };

        foreach (var name in commonIdNames)
        {
            if (props.Any(p => p.Name == name))
                return name;
        }

        // Fallback: tìm property có chứa "Ma" hoặc "Id"
        var idProp = props.FirstOrDefault(p =>
            p.Name.Contains("Ma") || p.Name.Contains("Id"));

        return idProp?.Name ?? "Id";
    }
    private string GetDisplayIdColumnName()
    {
        // Map từ property name sang display column name
        var mapping = new Dictionary<string, string>
        {
            { "MaNguoiDung", "MaNV" },
            { "MaMon", "MaMon" },
            { "MaBan", "MaBan" },
            { "MaHoaDon", "MaHD" }
        };

        return mapping.ContainsKey(_idPropertyName)
            ? mapping[_idPropertyName]
            : _idPropertyName;
    }
    private int GetItemId(T item)
    {
        var prop = typeof(T).GetProperty(_idPropertyName);
        return prop != null ? Convert.ToInt32(prop.GetValue(item)) : 0;
    }
    public async Task<bool> RefreshAsync()
    {
        return await LoadDataAsync();
    }
    public void FilterLocal(Func<T, bool> predicate)
    {
        try
        {
            var filteredData = _cachedSourceList
                .Where(predicate)
                .Select(_mapToDisplayModel)
                .ToList();

            _gridView.DataSource = filteredData;
            _gridView.ClearSelection();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi lọc dữ liệu: {ex.Message}");
        }
    }

    public int GetRowCount()
    {
        return _gridView.Rows.Count;
    }

    public bool HasSelection()
    {
        return _gridView.SelectedRows.Count > 0;
    }
}