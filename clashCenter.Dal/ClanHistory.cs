//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace clashCenter.Dal
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClanHistory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClanHistory()
        {
            this.ClanHistoryMembers = new HashSet<ClanHistoryMember>();
        }
    
        public int ClanHistoryID { get; set; }
        public Nullable<int> ClanID { get; set; }
        public string ClanName { get; set; }
        public Nullable<int> LocationID { get; set; }
        public int ClanLevel { get; set; }
        public int ClanPoints { get; set; }
        public int ClanVersusPoints { get; set; }
        public int MemberCount { get; set; }
        public string ClanType { get; set; }
        public int RequiredTrophies { get; set; }
        public string WarFrequency { get; set; }
        public int WarWinStreak { get; set; }
        public int WarWins { get; set; }
        public int WarTies { get; set; }
        public int WarLosses { get; set; }
        public bool IsWarLogPublic { get; set; }
        public string Description { get; set; }
        public System.DateTime DatePolled { get; set; }
    
        public virtual Clan Clan { get; set; }
        public virtual Location Location { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClanHistoryMember> ClanHistoryMembers { get; set; }
    }
}
