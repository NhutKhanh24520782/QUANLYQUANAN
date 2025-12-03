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

    // ==================== PHƯƠNG THỨC MỚI CẦN BỔ SUNG ====================

    public T GetItemAtRow(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= _gridView.Rows.Count)
            return default(T);

        if (rowIndex >= _cachedSourceList.Count)
            return default(T);

        // Đơn giản: Dựa vào index trực tiếp
        return _cachedSourceList[rowIndex];
    }

    public T GetSelectedItem()
    {
        if (_gridView.SelectedRows.Count == 0 || _gridView.CurrentRow == null)
            return default(T);

        int selectedIndex = _gridView.CurrentRow.Index;

        if (selectedIndex >= 0 && selectedIndex < _cachedSourceList.Count)
            return _cachedSourceList[selectedIndex];

        return default(T);
    }

    /// <summary>
    /// Lấy index của item trong cached list
    /// </summary>
    public int GetItemIndex(T item)
    {
        if (item == null) return -1;

        var itemId = GetItemId(item);
        for (int i = 0; i < _cachedSourceList.Count; i++)
        {
            if (GetItemId(_cachedSourceList[i]) == itemId)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Lấy danh sách tất cả items đang hiển thị
    /// </summary>
    public List<T> GetAllDisplayedItems()
    {
        var displayedItems = new List<T>();

        for (int i = 0; i < _gridView.Rows.Count; i++)
        {
            var item = GetItemAtRow(i);
            if (item != null)
                displayedItems.Add(item);
        }

        return displayedItems;
    }

    /// <summary>
    /// Tìm item theo điều kiện
    /// </summary>
    public T FindItem(Func<T, bool> predicate)
    {
        return _cachedSourceList.FirstOrDefault(predicate);
    }

    /// <summary>
    /// Tìm tất cả items theo điều kiện
    /// </summary>
    public List<T> FindAllItems(Func<T, bool> predicate)
    {
        return _cachedSourceList.Where(predicate).ToList();
    }

    /// <summary>
    /// Lấy item theo ID
    /// </summary>
    public T GetItemById(int id)
    {
        return _cachedSourceList.FirstOrDefault(item => GetItemId(item) == id);
    }

    /// <summary>
    /// Cập nhật item trong cache và refresh grid
    /// </summary>
    public bool UpdateItem(T updatedItem)
    {
        if (updatedItem == null) return false;

        var itemId = GetItemId(updatedItem);
        var existingIndex = _cachedSourceList.FindIndex(item => GetItemId(item) == itemId);

        if (existingIndex >= 0)
        {
            _cachedSourceList[existingIndex] = updatedItem;
            RefreshGridView();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Xóa item khỏi cache và refresh grid
    /// </summary>
    public bool RemoveItem(int id)
    {
        var removedCount = _cachedSourceList.RemoveAll(item => GetItemId(item) == id);

        if (removedCount > 0)
        {
            RefreshGridView();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Thêm item mới vào cache và refresh grid
    /// </summary>
    public void AddItem(T newItem)
    {
        if (newItem == null) return;

        _cachedSourceList.Add(newItem);
        RefreshGridView();
    }

    /// <summary>
    /// Thêm nhiều items mới
    /// </summary>
    public void AddRange(List<T> newItems)
    {
        if (newItems == null || newItems.Count == 0) return;

        _cachedSourceList.AddRange(newItems);
        RefreshGridView();
    }

    /// <summary>
    /// Xóa tất cả items
    /// </summary>
    public void ClearAll()
    {
        _cachedSourceList.Clear();
        RefreshGridView();
    }

    /// <summary>
    /// Refresh grid view từ cached data
    /// </summary>
    private void RefreshGridView()
    {
        if (_gridView == null) return;

        var displayData = _cachedSourceList.Select(_mapToDisplayModel).ToList();

        Action updateAction = () =>
        {
            _gridView.SuspendLayout();
            try
            {
                var currentSelection = GetSelectedItem();

                _gridView.DataSource = null;
                _gridView.DataSource = displayData;
                _gridView.ClearSelection();

                // Restore selection nếu có
                if (currentSelection != null)
                {
                    var id = GetItemId(currentSelection);
                    SelectById(id);
                }
            }
            finally
            {
                _gridView.ResumeLayout();
            }
        };

        if (_gridView.InvokeRequired)
        {
            _gridView.Invoke(updateAction);
        }
        else
        {
            updateAction();
        }
    }

    /// <summary>
    /// Lấy tên cột hiển thị cho một property
    public string GetDisplayColumnName(string propertyName)
    {
        // Tìm cột có DataPropertyName hoặc Name tương ứng
        foreach (DataGridViewColumn column in _gridView.Columns)
        {
            if (column.DataPropertyName == propertyName ||
                column.Name == propertyName)
                return column.HeaderText ?? column.Name;
        }
        return propertyName;
    }
    /// <summary>
    /// Sắp xếp grid theo cột
    /// </summary>
    public void SortByColumn(string columnName, bool ascending = true)
    {
        if (_gridView.DataSource == null) return;

        try
        {
            var dataView = (_gridView.DataSource as List<object>)?.AsQueryable();
            if (dataView == null) return;

            // Implementation tùy thuộc vào kiểu dữ liệu
            // Có thể cần custom sorter
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi SortByColumn: {ex.Message}");
        }
    }

    /// <summary>
    /// Đếm số items theo điều kiện
    /// </summary>
    public int Count(Func<T, bool> predicate = null)
    {
        if (predicate == null)
            return _cachedSourceList.Count;

        return _cachedSourceList.Count(predicate);
    }

    /// <summary>
    /// Kiểm tra có item nào thỏa điều kiện không
    /// </summary>
    public bool Any(Func<T, bool> predicate = null)
    {
        if (predicate == null)
            return _cachedSourceList.Any();

        return _cachedSourceList.Any(predicate);
    }

    // ==================== CÁC PHƯƠNG THỨC GỐC ====================

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

            if (_gridView.InvokeRequired)
            {
                _gridView.Invoke(new Action(() =>
                {
                    _gridView.DataSource = displayData;
                    _gridView.ClearSelection();
                }));
            }
            else
            {
                _gridView.DataSource = displayData;
                _gridView.ClearSelection();
            }

            return true;
        }
        catch (Exception ex)
        {
            // Log chi tiết hơn
            Console.WriteLine($"❌ Lỗi LoadDataAsync: {ex.Message}\n{ex.StackTrace}");

            // Show message trên UI thread
            if (_gridView.InvokeRequired)
            {
                _gridView.Invoke(new Action(() =>
                {
                    MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }

            return false;
        }
        finally
        {
            if (_gridView.InvokeRequired)
            {
                _gridView.Invoke(new Action(() =>
                {
                    _gridView.Enabled = true;
                    _gridView.Cursor = Cursors.Default;
                }));
            }
            else
            {
                _gridView.Enabled = true;
                _gridView.Cursor = Cursors.Default;
            }
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
        // Tìm trong cached list
        int index = _cachedSourceList.FindIndex(item => GetItemId(item) == id);

        if (index >= 0 && index < _gridView.Rows.Count)
        {
            _gridView.Rows[index].Selected = true;
            _gridView.FirstDisplayedScrollingRowIndex = index;
        }
    }
    private string DetectIdPropertyName()
    {
        var props = typeof(T).GetProperties();

        // Ưu tiên property có type là int và tên chứa "Id" hoặc "Ma"
        var idProp = props.FirstOrDefault(p =>
            p.PropertyType == typeof(int) || p.PropertyType == typeof(int?) &&
            (p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) ||
             p.Name.StartsWith("Ma", StringComparison.OrdinalIgnoreCase) ||
             p.Name.Contains("Ma") ||
             p.Name.Contains("Id")));

        if (idProp != null)
            return idProp.Name;

        // Fallback: property int đầu tiên
        idProp = props.FirstOrDefault(p => p.PropertyType == typeof(int) || p.PropertyType == typeof(int?));

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
            { "MaHoaDon", "MaHoaDon" },
            { "MaDonHang", "Mã ĐH" },
            { "MaChiTiet", "Mã CT" },
            { "Id", "ID" }
        };

        return mapping.ContainsKey(_idPropertyName)
            ? mapping[_idPropertyName]
            : _idPropertyName;
    }
    private Func<T, int> _getIdFunc;
    private Func<T, int> GetIdFunc
    {
        get
        {
            if (_getIdFunc == null)
            {
                var prop = typeof(T).GetProperty(_idPropertyName);
                if (prop == null)
                {
                    _getIdFunc = item => 0;
                }
                else
                {
                    _getIdFunc = item =>
                    {
                        var value = prop.GetValue(item);
                        return value != null ? Convert.ToInt32(value) : 0;
                    };
                }
            }
            return _getIdFunc;
        }
    }

    private int GetItemId(T item)
    {
        return GetIdFunc(item);
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

    public void UpdateDataSource(List<T> newData)
    {
        if (newData == null)
        {
            Console.WriteLine("⚠️ Dữ liệu mới là null, không thể cập nhật");
            return;
        }

        _cachedSourceList = newData;

        Action updateAction = () =>
        {
            var displayData = _cachedSourceList.Select(_mapToDisplayModel).ToList();

            // Giữ selection hiện tại
            var selectedItem = GetSelectedItem();

            _gridView.DataSource = null;
            _gridView.DataSource = displayData;

            // Khôi phục selection nếu có
            if (selectedItem != null)
            {
                var index = _cachedSourceList.IndexOf(selectedItem);
                if (index >= 0 && index < _gridView.Rows.Count)
                {
                    _gridView.Rows[index].Selected = true;
                }
            }

            Console.WriteLine($"✅ Đã cập nhật {newData.Count} items lên DataGridView");
        };

        if (_gridView.InvokeRequired)
        {
            _gridView.Invoke(updateAction);
        }
        else
        {
            updateAction();
        }
    }
    /// <summary>
    /// Lấy tên các cột hiện tại
    /// </summary>
    public List<string> GetColumnNames()
    {
        var columns = new List<string>();
        foreach (DataGridViewColumn column in _gridView.Columns)
        {
            columns.Add(column.Name);
        }
        return columns;
    }

    /// <summary>
    /// Ẩn/Hiện cột
    /// </summary>
    public void SetColumnVisibility(string columnName, bool visible)
    {
        if (_gridView.Columns.Contains(columnName))
        {
            _gridView.Columns[columnName].Visible = visible;
        }
    }

    /// <summary>
    /// Đặt độ rộng cột
    /// </summary>
    public void SetColumnWidth(string columnName, int width)
    {
        if (_gridView.Columns.Contains(columnName))
        {
            _gridView.Columns[columnName].Width = width;
        }
    }
}