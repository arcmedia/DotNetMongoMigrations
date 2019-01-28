using System.Linq;
using MongoDB.Bson;

namespace MongoMigrations
{
    /// <summary>
    /// Extension methods for BsonDocuments
    /// </summary>
    public static class BsonDocumentExtensions
    {
        /// <summary>
        /// Rename all instances of a name in a bson document to the new name.
        /// </summary>
        /// <param name="bsonDocument">The bson document.</param>
        /// <param name="originalName">Name of the original.</param>
        /// <param name="newName">The new name.</param>
        public static void ChangeName(this BsonDocument bsonDocument, string originalName, string newName)
        {
            var elements = bsonDocument.Elements
                .Where(e => e.Name == originalName)
                .ToList();
            foreach (var element in elements)
            {
                bsonDocument.RemoveElement(element);
                bsonDocument.Add(new BsonElement(newName, element.Value));
            }
        }

        /// <summary>
        /// Tries the get document identifier.
        /// </summary>
        /// <param name="bsonDocument">The bson document.</param>
        /// <returns>Document identifier</returns>
        public static object TryGetDocumentId(this BsonDocument bsonDocument)
        {
            BsonValue id;
            bsonDocument.TryGetValue("_id", out id);
            return id ?? "Cannot find id";
        }
    }
}