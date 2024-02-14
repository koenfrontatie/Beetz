using UnityEngine;
using System.Threading.Tasks;

public static class Utils
{
    public async static Task LerpToTarget(GameObject obj, Vector3 targetPosition, float duration)
    {
        var LerpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        Vector3 initialPosition = obj.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curveValue = LerpCurve.Evaluate(t);
            obj.transform.position = Vector3.Lerp(initialPosition, targetPosition, curveValue);
            await Task.Yield();
            elapsedTime += Time.deltaTime;
        }

        obj.transform.position = targetPosition;
    }
}
