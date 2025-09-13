// Services/PrologService.cs
using SbsSW.SwiPlCs;
using System;
using System.Collections.Generic;

namespace hechuyengia.Services
{
    public class PrologService
    {
        private static readonly object _lock = new();
        private static bool _isInited = false;

        // Dùng helper hiện có của bạn
        private readonly PrologHelper _prolog = new PrologHelper();

        public PrologService()
        {
            EnsureInit();
        }

        private static void EnsureInit()
        {
            lock (_lock)
            {
                if (!_isInited && !PlEngine.IsInitialized)
                {
                    // Khởi tạo yên lặng; thêm tham số đường dẫn .pl nếu cần
                    PlEngine.Initialize(new[] { "-q" });
                    _isInited = true;
                }
            }
        }

        public List<string> GetSymptoms()
        {
            lock (_lock)
            {
                return _prolog.GetSymptoms(); // đã có trong PrologHelper của bạn
            }
        }

        // Nếu PrologHelper có hàm Diagnose(List<string>), bọc luôn:
        public string Diagnose(List<string> s) => "unknown";

    }
}
