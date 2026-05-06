using UnityEngine;

[CreateAssetMenu(fileName = "DataTanah", menuName = "GreenEconomy/Data Tanah")]
public class SoilProperty : ScriptableObject
{
	public string namaWilayah;
	[Range(0, 100)] public float kelembapan;
	[Range(0, 100)] public float nutrisi;
	public string statusSuhu;
	public Color warnaSuhu;
}