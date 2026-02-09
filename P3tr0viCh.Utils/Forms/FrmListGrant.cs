using System;

namespace P3tr0viCh.Utils.Forms
{
    [Flags]
    public enum FrmListGrant
    {
        None = 0,
        Add = 1,
        Change = 2,
        Delete = 4,
        Sort = 8,
        MultiChange = 16,
        MultiDelete = 32,
        Default = Add | Change | Delete | Sort,
    }
}