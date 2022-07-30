using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGuidanceLine : MonoBehaviour
{
    [Header("Line Properties")]
	[SerializeField]
    int lineWidth = 3;
    [SerializeField]
	Material lineMaterial;
    
    Camera cam;
    public Vector3[] vertexPositions;

	void Awake()
	{
		cam = GetComponent<Camera>();
	}

    void Start()
    {
        vertexPositions = new Vector3[2];
    }

    void OnPostRender()
    {
        if (vertexPositions == null || vertexPositions.Length < 2)
			return;
 
		float nearClip = cam.nearClipPlane + 0.00001f;
		int end = vertexPositions.Length - 1;
		float thisWidth = 1f/Screen.width * lineWidth * 0.5f;
 
		lineMaterial.SetPass(0);

		if (lineWidth == 1)
		{
	        GL.Begin(GL.LINES);
	        for (int i = 0; i < end; ++i)
			{
                Vector2 linePoint = cam.WorldToViewportPoint(vertexPositions[i]);
                Vector2 nextlinePoint = cam.WorldToViewportPoint(vertexPositions[i + 1]);
                
	            GL.Vertex(cam.ViewportToWorldPoint(new Vector3(linePoint.x, linePoint.y, nearClip)));
	            GL.Vertex(cam.ViewportToWorldPoint(new Vector3(nextlinePoint.x, nextlinePoint.y, nearClip)));
        	}
    	}
    	else
		{
	        GL.Begin(GL.QUADS);
	        for (int i = 0; i < end; ++i)
			{
                Vector2 linePoint = cam.WorldToViewportPoint(vertexPositions[i]);
                Vector2 nextlinePoint = cam.WorldToViewportPoint(vertexPositions[i + 1]);

	            Vector3 perpendicular = (new Vector3(nextlinePoint.y, linePoint.x, nearClip) -
	                                 new Vector3(linePoint.y, nextlinePoint.x, nearClip)).normalized * thisWidth;
	            Vector3 v1 = new Vector3(linePoint.x, linePoint.y, nearClip);
	            Vector3 v2 = new Vector3(nextlinePoint.x, nextlinePoint.y, nearClip);
	            GL.Vertex(cam.ViewportToWorldPoint(v1 - perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v1 + perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v2 + perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v2 - perpendicular));
        	}
    	}
    	GL.End();
    }
}
