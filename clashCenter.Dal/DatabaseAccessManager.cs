﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using clashCenter.Dal.Models.ClashResponse;

namespace clashCenter.Dal
{
    public class DatabaseAccessManager
    {
        private List<Location> _locations;

        public DatabaseAccessManager()
        {
            using (var dbContxt = new ClashCenterEntities())
            {
                _locations = dbContxt.Locations.ToList();
            }
        }

        #region Create
        public void SaveClanInfo(Models.ClashResponse.Clan clan)
        {
            using (var dbContext = new ClashCenterEntities())
            {
                //try fetch clan from db, add new clan record is not
                Clan existingClan = dbContext.Clans.FirstOrDefault(c => c.ClanTag == clan.Tag);
                if (existingClan == null)
                {
                    existingClan = dbContext.Clans.Add(new Clan
                    {
                        ClanTag = clan.Tag,
                        FirstPolled = DateTime.Now
                    });
                    dbContext.SaveChanges();//save to get clanID
                }

                //location data is abit screwy because of location import
                var location = _locations.FirstOrDefault(l => l.Name == clan.Location?.name);

                
                var clanHistory = dbContext.ClanHistories.Add(new ClanHistory
                {
                    ClanID = existingClan.ClanID,
                    ClanName = clan.Name,
                    LocationID = location?.LocationID,
                    ClanLevel = clan.ClanLevel,
                    ClanPoints = clan.ClanPoints,
                    ClanVersusPoints = clan.ClanVersusPoints,
                    MemberCount = clan.Members,
                    ClanType = clan.Type,
                    RequiredTrophies = clan.RequiredTrophies,
                    WarFrequency = clan.WarFrequency,
                    WarWinStreak = clan.WarWinStreak,
                    WarWins = clan.WarWins,
                    WarTies = clan.WarTies,
                    WarLosses = clan.WarLosses,
                    IsWarLogPublic = clan.IsWarLogPublic,
                    Description = clan.Description,
                    DatePolled = DateTime.Now
                });

                //save to get clanhistoryid
                dbContext.SaveChanges();

                foreach (var member in clan.MemberList)
                {
                    dbContext.ClanHistoryMembers.Add(new ClanHistoryMember
                    {
                        ClanHistoryID = clanHistory.ClanHistoryID,
                        MemberTag = member.Tag,
                        MemberName = member.Name,
                        ExpLevel = member.ExpLevel,
                        Trophies = member.Trophies,
                        VersusTrophies = member.VersusTrophies,
                        ClanRole = member.Role,
                        ClanRank = member.ClanRank,
                        PreviousClanRank = member.PreviousClanRank,
                        Donations = member.Donations,
                        DonationsRecieved = member.DonationsReceived
                    });

                    //save each member
                    dbContext.SaveChanges();
                }
            }
        }

        public List<Models.ClashResponse.Clan> GetClanHistory(string tag)
        {
            using (var dbContext = new ClashCenterEntities())
            {
                var retVal = new List<Models.ClashResponse.Clan>();
                var allHistory = dbContext.ClanHistories.Where(ch => ch.Clan.ClanTag == tag).ToList();
                foreach (var history in allHistory)
                {
                    var newHistory = new Models.ClashResponse.Clan
                    {
                        ClanLevel = history.ClanLevel,
                        ClanPoints = history.ClanPoints,
                        ClanVersusPoints = history.ClanVersusPoints,
                        Members = history.MemberCount,
                        Type = history.ClanType,
                        RequiredTrophies = history.RequiredTrophies,
                        WarFrequency = history.WarFrequency,
                        WarWinStreak = history.WarWinStreak,
                        WarWins = history.WarWins,
                        WarTies = history.WarTies,
                        WarLosses = history.WarLosses,
                        IsWarLogPublic = history.IsWarLogPublic,
                        Description = history.Description,
                        MemberList = new List<ClanMember>()
                    };
                    
                    foreach (var clanMember in history.ClanHistoryMembers)
                    {
                        newHistory.MemberList.Add(new ClanMember
                        {
                            Tag = clanMember.MemberTag,
                            Name = clanMember.MemberName,
                            ExpLevel = clanMember.ExpLevel,
                            Trophies = clanMember.Trophies,
                            VersusTrophies = clanMember.VersusTrophies,
                            Role = clanMember.ClanRole,
                            ClanRank = clanMember.ClanRank,
                            PreviousClanRank = clanMember.PreviousClanRank,
                            Donations = clanMember.Donations,
                            DonationsReceived = clanMember.DonationsRecieved
                        });
                    }
                    retVal.Add(newHistory);
                }

                return retVal;
            }
        }
        #endregion

        #region Read
        public List<Location> GetLocations()
        {
            using (var dbContext = new ClashCenterEntities())
            {
                return dbContext.Locations.ToList();
            }
        }

        public List<Favorite> GetFavorites(string userId)
        {
            using (var dbContext = new ClashCenterEntities())
            {
                return dbContext.Favorites.Where(f => f.UserID == userId && f.Deleted == false).ToList();
            }
        }
        #endregion

        #region Update
        public bool AddFavorite(string tag, string userId)
        {
            using (var dbContext = new ClashCenterEntities())
            {
                if (!dbContext.CurrentFavorites.Any(f => f.ClashTargetId == tag && f.UserId == userId))
                {
                    dbContext.Favorites.Add(new Favorite()
                    {
                        ClashTargetID = tag,
                        UserID = userId,
                        Deleted = false,
                        CreatedDate = DateTime.Now,
                        IsInterest = false
                    });
                    return dbContext.SaveChanges() != 0;
                }
                return false;
            }
        }

        public bool AddInterest(string tag, string userId)
        {
            using (var dbContext = new ClashCenterEntities())
            {
                if (!dbContext.CurrentFavorites.Any(f => f.ClashTargetId == tag && f.UserId == userId))
                {
                    dbContext.Favorites.Add(new Favorite()
                    {
                        ClashTargetID = tag,
                        UserID = userId,
                        Deleted = false,
                        CreatedDate = DateTime.Now,
                        IsInterest = true
                    });
                    return dbContext.SaveChanges() != 0;
                } else
                {
                    dbContext.Favorites.FirstOrDefault(f => f.ClashTargetID == tag && f.UserID == userId).IsInterest = true;
                    return dbContext.SaveChanges() != 0;
                }
            }
        }

        public bool RemoveFavorite(string tag, string userId)
        {
            using (var dbContext = new ClashCenterEntities())
            {
                if (dbContext.Favorites.Any(f => f.ClashTargetID == tag && f.UserID == userId))
                {
                    foreach (var favorite in dbContext.Favorites.Where(f => f.ClashTargetID == tag && f.UserID == userId))
                    {
                        favorite.Deleted = true;
                    }
                    return dbContext.SaveChanges() != 0;
                }
                return false;
            }
        }

        public bool RemoveInterest(string tag, string userId)
        {
            using (var dbContext = new ClashCenterEntities())
            {
                if (dbContext.Favorites.Any(f => f.ClashTargetID == tag && f.UserID == userId))
                {
                    foreach (var favorite in dbContext.Favorites.Where(f => f.ClashTargetID == tag && f.UserID == userId))
                    {
                        favorite.IsInterest = false;
                    }
                    return dbContext.SaveChanges() != 0;
                }
                return false;
            }
        }
        #endregion
    }
}
