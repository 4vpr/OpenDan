namespace OpenDan.ChartManager
{
    public class ChartManager
    {
        public Chart? selected_chart;
        public Chart[] chart_list = [];
        public void StartParse()
        {

        }
        public (Chart, bool) ParseBeatMapFromPath(string path)
        {
            Chart chart = new Chart();
            return (chart, true);
        }
    }
}