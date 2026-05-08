using UnityEngine;

[CreateAssetMenu(fileName = "NewSoilProperty", menuName = "Environment/Soil Property")]
public class SoilProperty : ScriptableObject
{
	public string namaWilayah;
	public float kelembapan;
	public float nutrisi;
	public float suhu; // Input angka derajat di sini

	// Otomatis menentukan teks status suhu
	public string statusSuhu
	{
		get
		{
			if (suhu <= 22) return "DINGIN";
			if (suhu >= 23 && suhu <= 30) return "SEDANG";
			return "PANAS";
		}
	}

	// Otomatis menentukan warna teks suhu
	public Color warnaSuhu
	{
		get
		{
			if (suhu <= 22) return Color.cyan;
			if (suhu >= 23 && suhu <= 30) return Color.green;
			return new Color(1f, 0.5f, 0f); // Oranye
		}
	}
}