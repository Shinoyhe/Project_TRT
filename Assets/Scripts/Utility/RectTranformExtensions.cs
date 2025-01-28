using UnityEngine;

// Code from user oscarAbraham on the Unity Forums:
// https://discussions.unity.com/t/why-is-rect-overlaps-not-working/252658/2
public static class RectTransformExtensions
{ 
    // Use this method like this: RectTransformExtensions.GetWorldRect(someRectTransform);

    /// <summary>
    /// Transforms our rect from local space to global space. Allows us to check if rects with
    /// different parents overlap with each other.
    /// </summary>
    /// <param name="rectTransform">RectTransform - the local rect to convert.</param>
    /// <returns>RectTransform - the converted global rect.</returns>
    public static Rect GetWorldRect(this RectTransform rectTransform)
    {
        Rect localRect = rectTransform.rect;

        return new Rect
        {
            min = rectTransform.TransformPoint(localRect.min),
            max = rectTransform.TransformPoint(localRect.max)
        };
    }
}