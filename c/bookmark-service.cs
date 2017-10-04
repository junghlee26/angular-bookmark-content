using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlakWealth.Data.Providers;
using BlakWealth.Models.Requests;
using System.Data.SqlClient;
using BlakWealth.Services.Interfaces;
using System.Data;
using BlakWealth.Models.Domain;
using BlakWealth.Data;

namespace BlakWealth.Services
{
    public class BookmarkService : IBookmarkService
    {
        readonly IDataProvider dataProvider;
        public BookmarkService(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public int Insert(BookmarkAddRequest model)
        {
            int id = 0;

            dataProvider.ExecuteNonQuery("dbo.UserBookmark_Insert"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@UserId", model.UserId);
                paramCollection.AddWithValue("@ContentId", model.ContentId);

                SqlParameter idParameter = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                idParameter.Direction = System.Data.ParameterDirection.Output;

                paramCollection.Add(idParameter);

            }, returnParameters: delegate (SqlParameterCollection param)
            {
                id = (int)param["@Id"].Value;
            }
            );
            return id;
        }


        public List<Bookmark> GetByUserId(int userId, int itemNum, int pageNum)
        {
            List<Bookmark> list = null;

            dataProvider.ExecuteCmd("dbo.UserBookmark_SelectByUserId"
                  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                  {
                      paramCollection.AddWithValue("@UserId", userId);
                      paramCollection.AddWithValue("@itemNum", itemNum);
                      paramCollection.AddWithValue("@pageNum", pageNum);
                  }
               , singleRecordMapper: delegate (IDataReader reader, short set)
               {
                   Bookmark singleItem = new Bookmark();
                   int startingIndex = 0; //startingOrdinal

                   singleItem.UserId = reader.GetSafeInt32(startingIndex++);
                   singleItem.ContentId = reader.GetSafeInt32(startingIndex++);
                   singleItem.DateAdded = reader.GetSafeUtcDateTime(startingIndex++);
                   singleItem.Id = reader.GetSafeInt32(startingIndex++);
                   singleItem.ContentTitle = reader.GetSafeString(startingIndex++);

                   if (list == null)
                   {
                       list = new List<Bookmark>();
                   }

                   list.Add(singleItem);
               }
               );
            return list;
        }


        public void DeleteByContentId(int userId, int contentId)
        {
            dataProvider.ExecuteNonQuery("dbo.UserBookmark_DeleteByContentId"
             , inputParamMapper: delegate (SqlParameterCollection paramCollection)
             {
                 paramCollection.AddWithValue("@UserId", userId);
                 paramCollection.AddWithValue("@ContentId", contentId);
             });
        }


        //GET: REQUEST BOOKMARK STATUS//

        public Dictionary<int, Bookmark> GetBookmarkRequest(int userId, int[] contentIds)
        {
            var results = new Dictionary<int, Bookmark>();

            dataProvider.ExecuteCmd("dbo.UserBookmark_Check",
                 parameters =>
                 {
                     parameters.AddWithValue("@UserId", userId);
                     var p = parameters.AddWithValue("@ids", new IntIdTable(contentIds));
                     p.SqlDbType = System.Data.SqlDbType.Structured;
                     p.TypeName = "dbo.IntIdTable";
                 },
                 (reader, set) =>
                 {
                     int contentId = reader.GetSafeInt32(1);
                     results[contentId] = new Bookmark
                     {
                         Id = reader.GetSafeInt32(0),
                         ContentId = contentId
                     };
                 });

            return results;
        }
    }
}
