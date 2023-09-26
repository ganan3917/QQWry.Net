using System.Reflection;
using System.Text;

namespace qqwry;

/// <summary>
///     IP工具类
/// </summary>
internal sealed class IpUtils
{
    private const    int                      _CAPACITY   = 600000;
    private readonly List<uint>               _arrStart   = new(_CAPACITY);
    private readonly Dictionary<uint, string> _ipDatabase = new(_CAPACITY);

    /// <summary>
    ///     Initializes a new instance of the <see cref="IpUtils" /> class.
    /// </summary>
    public IpUtils()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using var sr = new StreamReader( //
            Assembly.GetExecutingAssembly().GetManifestResourceStream("qqwry.qqwrt-20230920.txt")!
          , Encoding.GetEncoding("gbk"));

        while (!sr.EndOfStream) {
            var lineArr = sr.ReadLine()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var start   = IpV4ToUInt32(lineArr[0]);
            _arrStart.Add(start);
            _ipDatabase.Add(start, string.Join(' ', lineArr[2..]).Replace("CZ88.NET", string.Empty).Trim());
        }
    }

    /// <summary>
    ///     数据量
    /// </summary>
    public int DataCount => _ipDatabase.Count;

    /// <summary>
    ///     IP地址转UInt32
    /// </summary>
    public static uint IpV4ToUInt32(string ipStr)
    {
        return BitConverter.ToUInt32(ipStr.Split('.').Select(byte.Parse).Reverse().ToArray(), 0);
    }

    /// <summary>
    ///     查询IP归属地
    /// </summary>
    public string Query(uint ip)
    {
        var index = FindStartIndex(ip);
        return _ipDatabase[_arrStart[index]];
    }

    private int FindStartIndex(uint ip)
    {
        var left  = 0;
        var right = _arrStart.Count - 1;
        var ret   = -1;

        while (left <= right) {
            // ReSharper disable once ArrangeRedundantParentheses
            var mid = left + ((right - left) / 2);

            if (_arrStart[mid] <= ip) {
                ret  = mid;     // 更新最大元素的下标
                left = mid + 1; // 调整左边界
            }
            else {
                right = mid - 1; // 调整右边界
            }
        }

        return ret;
    }
}