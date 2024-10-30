using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class IPDFetcher : MonoBehaviour
{
    [SerializeField] private string ip = "192.168.178.60";
    private string endpoint = "/api/holographic/os/settings/ipd";
    [SerializeField] private string username = "user";
    [SerializeField] private string password = "pw";

    private IPD ipd;
    private string url => $"https://{ip}{endpoint}";

    [System.Serializable]
    private class IPD
    {
        public float ipd;
        public string ipdMM;
    }

    [ContextMenu("Fetch IPD")]
    public void FetchIPD() => StartCoroutine(FetchIPDCoroutine());

    private IEnumerator FetchIPDCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // ignore ssl
            request.certificateHandler = new BypassCertificate();

            // basic auth
            string authInfo = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));
            request.SetRequestHeader("Authorization", "Basic " + authInfo);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) ProcessIPD(request.downloadHandler.text);
            else Debug.LogError($"Error fetching IPD: {request.error}");
        }
    }

    private void ProcessIPD(string jsonData)
    {
        // converts raw IPD value to mm
        this.ipd = JsonUtility.FromJson<IPD>(jsonData);
        float ipdMM = ipd.ipd / 1000f;
        ipd.ipdMM = ipdMM.ToString("F2");
        Debug.Log($"IPD in mm: {ipd.ipdMM}mm | IPD raw: {ipd.ipd}");
    }

    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
