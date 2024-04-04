using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    // Index for speeds and sizes.
    [SerializeField] Transform[] backgrounds; // 0.
    [SerializeField] Transform[] midgrounds; // 1.
    [SerializeField] Transform[] foregrounds; // 2.

    [SerializeField] float[] moveSpeeds;

    private float[] sizes; // Width of bg type.
    private float[] backgroundStarts;
    private float[] midgroundStarts;
    private float[] foregroundStarts;

    void Start()
    {
        sizes = new float[3];
        sizes[0] = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
        sizes[1] = midgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
        sizes[2] = foregrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;

        backgroundStarts = new float[backgrounds.Length];
        for (int i = 0; i < backgroundStarts.Length; i++)
            backgroundStarts[i] = backgrounds[i].transform.position.x;

        midgroundStarts = new float[midgrounds.Length];
        for (int i = 0; i < midgroundStarts.Length; i++)
            midgroundStarts[i] = midgrounds[i].transform.position.x;

        foregroundStarts = new float[foregrounds.Length];
        for (int i = 0; i < foregroundStarts.Length; i++)
            foregroundStarts[i] = foregrounds[i].transform.position.x;
    }

    void FixedUpdate()
    {
        // Move the backgrounds.
        foreach (var bg in backgrounds)
            bg.Translate(moveSpeeds[0] * Time.fixedDeltaTime, 0.0f, 0.0f);
        foreach (var mg in midgrounds)
            mg.Translate(moveSpeeds[1] * Time.fixedDeltaTime, 0.0f, 0.0f);
        foreach (var fg in foregrounds)
            fg.Translate(moveSpeeds[2] * Time.fixedDeltaTime, 0.0f, 0.0f);

        // Check if backgrounds go off-screen.
        if (backgrounds[0].transform.position.x <= -sizes[0])
        {
            for (int i = 0; i < backgrounds.Length;i++)
                backgrounds[i].transform.position = new Vector3(backgroundStarts[i], 0.0f, 0.0f);
        }
        if (midgrounds[0].transform.position.x <= -sizes[1])
        {
            for (int i = 0; i < midgrounds.Length; i++)
                midgrounds[i].transform.position = new Vector3(midgroundStarts[i], 0.0f, 0.0f);
        }
        if (foregrounds[0].transform.position.x <= -sizes[2])
        {
            for (int i = 0; i < foregrounds.Length; i++)
                foregrounds[i].transform.position = new Vector3(foregroundStarts[i], -16f, 0.0f);
        }
    }
}
