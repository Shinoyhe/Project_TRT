using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Debug_LoopWipeUI : MonoBehaviour
{
    [SerializeField]
    private float duration = 0.5f;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        _image.fillAmount = 0;

        TimeLoopManager.LoopElapsed += AnimateWipe;
    }

    private void OnDestroy()
    {
        TimeLoopManager.LoopElapsed -= AnimateWipe;
    }

    private void AnimateWipe(System.Action callback)
    {
        if (callback == null) {
            Debug.LogError("Debug_LoopWipe Error: AnimateWipe failed. callback was null.");
            return;
        }

        StartCoroutine(AnimateWipeRoutine(callback));
    }

    private IEnumerator AnimateWipeRoutine(System.Action callback)
    {
        if (_image == null) {
            // Callback cannot be null, because of our AnimateWipe() check.
            callback();
        }

        float elapsed = 0;
        _image.fillAmount = 0;

        while (elapsed < duration) {
            _image.fillAmount = elapsed/duration;

            yield return null;
            elapsed += Time.deltaTime;
        }

        _image.fillAmount = 1;
        callback();
    }
}