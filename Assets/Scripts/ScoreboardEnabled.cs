using UnityEngine;
using TMPro;

public class ScoreboardEnabled : MonoBehaviour
{
    [SerializeField]
    private Spawner spawnerObject;

    [SerializeField]
    private TextMeshProUGUI[] soloStat;

    [SerializeField]
    private TextMeshProUGUI[] totalStat;

    private void OnEnable()
    {
        //solo stats
        soloStat[0].text = spawnerObject.soloObjectives.ToString();
        soloStat[1].text = spawnerObject.soloKills.ToString();
        soloStat[2].text = spawnerObject.soloRevives.ToString();
        soloStat[3].text = spawnerObject.soloDeaths.ToString();
        //total stats
        totalStat[0].text = spawnerObject.totalObjectives.ToString();
        totalStat[1].text = spawnerObject.totalKills.ToString();
        totalStat[2].text = spawnerObject.totalRevives.ToString();
        totalStat[3].text = spawnerObject.totalDeaths.ToString();
    }
}
