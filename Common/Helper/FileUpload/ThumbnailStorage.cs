using System.Collections.Generic;


namespace Common
{
    public class ThumbnailStorage
    {
        private static ThumbnailStorage _Instance;
        private Dictionary<string, List<Thumbnail>> _Thumbnails;
        public static ThumbnailStorage Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ThumbnailStorage();
                return _Instance;
            }
        }

        private ThumbnailStorage()
        {
            _Thumbnails = new Dictionary<string, List<Thumbnail>>();
        }

        public void Add(string sessionId, List<Thumbnail> thumbs)
        {
            if (_Thumbnails.ContainsKey(sessionId))
            {
                _Thumbnails.Remove(sessionId);
            }
            _Thumbnails.Add(sessionId, thumbs);
        }
        public List<Thumbnail> GetById(string sessionId)
        {
            if (_Thumbnails.ContainsKey(sessionId))
                return _Thumbnails[sessionId];
            return null;
        }
        public void DeleteById(string sessionId)
        {
            if (_Thumbnails.ContainsKey(sessionId))
                _Thumbnails.Remove(sessionId);
        }
    }
}