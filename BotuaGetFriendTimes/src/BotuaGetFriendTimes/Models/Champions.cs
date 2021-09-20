namespace BotuaGetFriendTimes.Models
{
    public class Champions
    {
        public Champion ActiveChampion { get; set; }
        public Champion Muted { get; set; }
        public Champion Deafened { get; set; }
        public Champion Afk { get; set; }
        public Champion Streaming { get; set; }
        public Champion Video { get; set; }
        
        public Champions(Champion activeChampion, Champion mutedChampion, Champion deafenedChampion, Champion afkChampion, Champion streamingChampion, Champion videoChampion)
        {
            ActiveChampion = activeChampion;
            Muted = mutedChampion;
            Deafened = deafenedChampion;
            Afk = afkChampion;
            Streaming = streamingChampion;
            Video = videoChampion;
        }
    }
}