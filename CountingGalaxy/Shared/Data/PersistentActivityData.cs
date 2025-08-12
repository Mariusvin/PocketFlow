namespace Activities.Shared.Data
{
    public static class PersistentActivityData
    {
        private static BaseActivityData currentActivityData;

        public static bool IsActivityDataSet => currentActivityData != null;

        public static void SetCurrentActivityData(BaseActivityData _activityData)
        {
            currentActivityData = _activityData;
        }

        public static bool TryConvertCurrentActivityData<T>(out T _convertedData) where T : BaseActivityData
        {
            _convertedData = null;
            if (currentActivityData is not T _data)
            {
                currentActivityData = null;
                return false;
            }

            _convertedData = _data;
            currentActivityData = null;
            return true;
        }
    }
}