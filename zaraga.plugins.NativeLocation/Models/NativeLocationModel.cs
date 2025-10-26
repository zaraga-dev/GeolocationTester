namespace zaraga.plugins.NativeLocation.Models
{
    public class NativeLocationModel
    {
        public NativeLocationModel(double latitude
            , double longitude
            , float accuracy
            , double altituyde
            , double speed
            , DateTime timeStamp
            , double verticalAccuracy
            , bool reducedAccuracy
            , double course
            , bool isFromMockProvider)
        {
            Latitude = latitude;
            Longitude = longitude;
            Accuracy = accuracy;
            Altitude = altituyde;
            Speed = speed;
            Timestamp = timeStamp;
            VerticalAccuracy = verticalAccuracy;
            ReducedAccuracy = reducedAccuracy;
            Course = course;
            IsFromMockProvider = isFromMockProvider;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float Accuracy { get; set; }
        public double Altitude { get; set; }
        public double Speed { get; set; }
        public DateTime Timestamp { get; set; }
        public double VerticalAccuracy { get; set; }
        public bool ReducedAccuracy { get; set; }
        public double Course { get; set; }
        public bool IsFromMockProvider { get; set; }
    }
}
