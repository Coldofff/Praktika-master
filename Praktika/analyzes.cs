//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Praktika
{
    using System;
    using System.Collections.Generic;
    
    public partial class analyzes
    {
        public int PK_analysis_id { get; set; }
        public System.DateTime date { get; set; }
        public int added_count { get; set; }
        public int deleted_count { get; set; }
        public int changed_count { get; set; }
        public Nullable<int> FK_user_id { get; set; }
    
        public virtual users users { get; set; }
    }
}