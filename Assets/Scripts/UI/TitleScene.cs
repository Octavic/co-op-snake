using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public FadeBlack Fade;
    private bool isLoading = false;

    // Update is called once per frame
    void Update()
    {
        if (!this.isLoading)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.isLoading = true;
                StartCoroutine(this.LoadScene());
            }
        }
    }

    private IEnumerator LoadScene()
    {
        this.Fade.ToBlack();
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(1);
    }
}
