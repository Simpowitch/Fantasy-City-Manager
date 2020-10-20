using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class Soft {
        
        public static Pair2D pair = Pair2D.Zero();

        public static void Draw(LightingBuffer2D buffer, List<Polygon2D> polygons, Vector2 scale) {
			Vector2 offset = ShadowEngine.lightOffset + ShadowEngine.objectOffset;
            float lightSizeSquared = ShadowEngine.lightSize * 2;
            float z = ShadowEngine.shadowZ;
            
			float lightSize = buffer.lightSource.size;
			float lightCoreSize = buffer.lightSource.coreSize;

			Rect penumbraRect = ShadowEngine.Penumbra.uvRect;
			Vector2 penumbraSize = ShadowEngine.Penumbra.size;


			// Intersection
			Vector2 edge_intersection = new Vector2();

			// Left
			Vector2 edge_world_left = new Vector2();

			float edge_left_cross_dir;
			Vector2 edge_left_cross;

			float edge_left_cross_left_dir;
			Vector2 edge_left_cross2;

			float core_left_90_dir;
			Vector2 core_left_90 = new Vector2();

			float core_left_90_edge_dir;
			Vector2 core_left_90_edge = new Vector2();

			float core_left_270_dir;
			Vector2 core_left_270 = new Vector2();

			float core_left_270_edge_dir;
			Vector2 core_left_270_edge = new Vector2();
			

			// Right
			Vector2 edge_world_right = new Vector2();

			float edge_right_cross_dir;
			Vector2 edge_right_cross;

			float edge_right_cross2_dir;
			Vector2 edge_right_cross2;

			float core_right_90_dir;
			Vector2 core_right_90 = new Vector2();

			float core_right_90_edge_dir;
			Vector2 core_right_90_edge = new Vector2();

			float core_right_270_dir;
			Vector2 core_right_270 = new Vector2();

			float core_right_270_edge_dir;
			Vector2 core_right_270_edge = new Vector2();
			
			for(int i = 0; i < polygons.Count; i++) {

				if (ShadowEngine.lightDrawAbove && polygons[i].PointInPoly (-offset)) {
					continue;
				}

				List<Vector2D> pointsList = polygons[i].pointsList;
				int pointsCount = pointsList.Count;
			
				 for(int x = 0; x < pointsCount; x++) {
					pair.A.x = pointsList[x].x;
                    pair.A.y = pointsList[x].y;

                    pair.B.x = pointsList[(x + 1) % pointsCount].x ;
                    pair.B.y = pointsList[(x + 1) % pointsCount].y ;

					Pair2D edge_local = pair;

					float p_a_x = (float)edge_local.A.x * scale.x;
					float p_a_y = (float)edge_local.A.y * scale.y;

					float p_b_x = (float)edge_local.B.x * scale.x;
					float p_b_y = (float)edge_local.B.y * scale.y;

					float pa_x = p_a_x + offset.x;
					float pa_y = p_a_y + offset.y;

					float pb_x = p_b_x + offset.x;
					float pb_y = p_b_y + offset.y;
					
					float lightDirection = Mathf.Atan2((pa_y + pb_y) / 2 , (pa_x + pb_x) / 2 ) * Mathf.Rad2Deg;
					float EdgeDirection = Mathf.Atan2(p_a_y - p_b_y, p_a_x - p_b_x) * Mathf.Rad2Deg - 180;

					lightDirection -= EdgeDirection;
	
					lightDirection = (lightDirection + 720) % 360;
					if (lightDirection < 180 ) {
						continue;
					}

					//////////////////////////////////////////////
					edge_world_left.x = (float)edge_local.B.x * scale.x + offset.x;
					edge_world_left.y = (float)edge_local.B.y * scale.y + offset.y;

					// Edge Cross
					edge_left_cross_dir = Mathf.Atan2(edge_world_left.y, edge_world_left.x);
					edge_left_cross.x = Mathf.Cos(edge_left_cross_dir) * lightSize;
					edge_left_cross.y = Mathf.Sin(edge_left_cross_dir) * lightSize;

					// Core 90 Degre Point
					core_left_90_dir = Mathf.Atan2(edge_world_left.y, edge_world_left.x) + Mathf.PI / 2;
					core_left_90.x = Mathf.Cos(core_left_90_dir) * lightCoreSize;
					core_left_90.y = Mathf.Sin(core_left_90_dir) * lightCoreSize;
					// Core 90 Degre To Edge
					core_left_90_edge_dir = Mathf.Atan2(edge_world_left.y - core_left_90.y, edge_world_left.x - core_left_90.x);
					core_left_90_edge.x = core_left_90.x + Mathf.Cos(core_left_90_edge_dir) * lightSize;
					core_left_90_edge.y = core_left_90.y + Mathf.Sin(core_left_90_edge_dir) * lightSize;

					// Core 270 Degre Point
					core_left_270_dir = Mathf.Atan2(edge_world_left.y, edge_world_left.x) - Mathf.PI / 2;
					core_left_270.x = Mathf.Cos(core_left_270_dir) * lightCoreSize;
					core_left_270.y = Mathf.Sin(core_left_270_dir) * lightCoreSize;

					// Core 270 Degre To Edge
					core_left_270_edge_dir = Mathf.Atan2(edge_world_left.y - core_left_270.y, edge_world_left.x - core_left_270.x);
					core_left_270_edge.x = core_left_270.x + Mathf.Cos(core_left_270_edge_dir) * lightSize;
					core_left_270_edge.y = core_left_270.y + Mathf.Sin(core_left_270_edge_dir) * lightSize;
					
					//////////////////////////////////////////////
					edge_world_right.x = (float)edge_local.A.x * scale.x + offset.x;
					edge_world_right.y = (float)edge_local.A.y * scale.y  + offset.y;

					// Edge Cross
					edge_right_cross_dir = Mathf.Atan2(edge_world_right.y, edge_world_right.x);
					edge_right_cross.x = Mathf.Cos(edge_right_cross_dir) * lightSize;
					edge_right_cross.y = Mathf.Sin(edge_right_cross_dir) * lightSize;
					
					// Core 90 Degre Point
					core_right_90_dir = Mathf.Atan2(edge_world_right.y, edge_world_right.x) - Mathf.PI / 2;
					core_right_90.x = Mathf.Cos(core_right_90_dir) * lightCoreSize;
					core_right_90.y = Mathf.Sin(core_right_90_dir) * lightCoreSize;

					// Core 90 Degre To Edge
					core_right_90_edge_dir = Mathf.Atan2(edge_world_right.y - core_right_90.y, edge_world_right.x - core_right_90.x);
					core_right_90_edge.x = core_right_90.x + Mathf.Cos(core_right_90_edge_dir) * lightSize;
					core_right_90_edge.y = core_right_90.y + Mathf.Sin(core_right_90_edge_dir) * lightSize;
					
					// Core 270 Degre Point
					core_right_270_dir = Mathf.Atan2(edge_world_right.y, edge_world_right.x) + Mathf.PI / 2;
					core_right_270.x = Mathf.Cos(core_right_270_dir) * lightCoreSize;
					core_right_270.y = Mathf.Sin(core_right_270_dir) * lightCoreSize;

					// Core 270 Degre To Edge
					core_right_270_edge_dir = Mathf.Atan2(edge_world_right.y - core_right_270.y, edge_world_right.x - core_right_270.x);
					core_right_270_edge.x = core_right_270.x + Mathf.Cos(core_right_270_edge_dir) * lightSize;
					core_right_270_edge.y = core_right_270.y + Mathf.Sin(core_right_270_edge_dir) * lightSize;

				
					// Object To Light
					//Draw(new Vector2D(edge_world_left), new Vector2D(edge_world_right), 0 );
					
					// LEFT
				
					//Draw(new Vector2D(edge_left_cross), Vector2D.Zero(), 1 );
					//Draw(new Vector2D(core_left_90), Vector2D.Zero(), 2);
					//Draw(new Vector2D(core_left_90), new Vector2D(core_left_90_edge), 3);
					//Draw(new Vector2D(core_left_270), Vector2D.Zero(), 4);
					//Draw(new Vector2D(core_left_270), new Vector2D(core_left_270_edge), 5);


					// RIGHT
					//Draw(new Vector2D(edge_right_cross), Vector2D.Zero(), 10 );
					//Draw(new Vector2D(core_right_90), Vector2D.Zero(), 11);
					//Draw(new Vector2D(core_right_90), new Vector2D(core_right_90_edge), 12);
					//Draw(new Vector2D(core_right_270), Vector2D.Zero(), 13);
					//Draw(new Vector2D(core_right_270), new Vector2D(core_right_270_edge), 14);

					GL.Color(Color.white);

					Vector2D intersection = Math2D.GetPointLineIntersectLine(new Pair2D(new Vector2D(core_left_90), new Vector2D(core_left_90_edge)), new Pair2D(new Vector2D(core_right_90), new Vector2D(core_right_90_edge)));
						
					if (intersection != null) {
						
						edge_intersection.x = (float)intersection.x;
						edge_intersection.y = (float)intersection.y;



						float mid_x = (core_left_90_edge.x + core_right_90_edge.x) / 2;
						float mid_y = (core_left_90_edge.y + core_right_90_edge.y) / 2;
						
						////////// MIDLE
						GL.TexCoord3(penumbraRect.width, penumbraRect.y+ penumbraRect.height  + penumbraSize.y / 2, 0);
						GL.Vertex3(core_left_90_edge.x, core_left_90_edge.y, z);

						GL.TexCoord3(penumbraRect.x, penumbraRect.height + penumbraSize.y / 2, 0);
						GL.Vertex3(mid_x, mid_y, z);

						GL.TexCoord3(penumbraRect.x, penumbraRect.y + penumbraSize.y / 2, 0);				
						GL.Vertex3(edge_intersection.x, edge_intersection.y, z);


						GL.TexCoord3(penumbraRect.width, penumbraRect.y+ penumbraRect.height  + penumbraSize.y / 2, 0);
						GL.Vertex3(core_right_90_edge.x, core_right_90_edge.y, z);

						GL.TexCoord3(penumbraRect.x, penumbraRect.height + penumbraSize.y / 2, 0);
						GL.Vertex3(mid_x, mid_y, z);

						GL.TexCoord3(penumbraRect.x, penumbraRect.y + penumbraSize.y / 2, 0);				
						GL.Vertex3(edge_intersection.x, edge_intersection.y, z);


						// Right Intersection

						edge_right_cross2_dir = edge_right_cross_dir - Mathf.PI / 2;
						edge_right_cross2.x = edge_intersection.x + Mathf.Cos(edge_right_cross2_dir) * lightSize * 2;
						edge_right_cross2.y = edge_intersection.y + Mathf.Sin(edge_right_cross2_dir) * lightSize * 2;
			
						Vector2D intersectionRight = Math2D.GetPointLineIntersectLine(new Pair2D(new Vector2D(edge_intersection), new Vector2D(edge_right_cross2)), new Pair2D(new Vector2D(core_right_270), new Vector2D(core_right_270_edge)));
						if (intersectionRight != null) {

							edge_right_cross2.x = (float)intersectionRight.x;
							edge_right_cross2.y = (float)intersectionRight.y;

							//Draw(new Vector2D(edge_intersection), new Vector2D(edge_right_cross2), 6 );

							// First Fin Right
							GL.TexCoord3(penumbraRect.x, penumbraRect.y, 0);
							GL.Vertex3(edge_world_right.x, edge_world_right.y, z);
						
							GL.TexCoord3(penumbraRect.width, penumbraRect.y + penumbraRect.height, 0);
							GL.Vertex3(edge_right_cross2.x, edge_right_cross2.y, z);

							GL.TexCoord3(penumbraRect.x, penumbraRect.height, 0);
							GL.Vertex3(edge_intersection.x, edge_intersection.y, z);

							// Second To Last Fin Right
							GL.TexCoord3(penumbraRect.width, penumbraRect.y+ penumbraRect.height  + penumbraSize.y / 2, 0);
							GL.Vertex3(core_left_90_edge.x, core_left_90_edge.y, z);

							GL.TexCoord3(penumbraRect.width, penumbraRect.y + penumbraRect.height, 0);
							GL.Vertex3(edge_right_cross2.x, edge_right_cross2.y, z);

							GL.TexCoord3(penumbraRect.x, penumbraRect.y + penumbraSize.y / 2, 0);	
							GL.Vertex3(edge_intersection.x, edge_intersection.y, z);


							// Last Fine Right
							GL.TexCoord3(penumbraRect.x+ penumbraSize.x / 2, penumbraRect.height + penumbraSize.y / 2, 0);
							GL.Vertex3(core_left_90_edge.x, core_left_90_edge.y, z);
							
							GL.TexCoord3(penumbraRect.x+ penumbraSize.x / 2, penumbraRect.y + penumbraSize.y / 2, 0);	
							GL.Vertex3(edge_right_cross2.x, edge_right_cross2.y, z);

							GL.TexCoord3(penumbraRect.width+ penumbraSize.x / 2, penumbraRect.y+ penumbraRect.height  + penumbraSize.y / 2, 0);
							GL.Vertex3(core_right_270_edge.x, core_right_270_edge.y, z);

							//Draw(new Vector2D(edge_right_cross2), new Vector2D(core_left_90_edge), 9 );
						}

						// Left Intersection

						edge_left_cross_left_dir = edge_left_cross_dir + Mathf.PI / 2;
						edge_left_cross2.x = edge_intersection.x + Mathf.Cos(edge_left_cross_left_dir) * lightSize * 2;
						edge_left_cross2.y = edge_intersection.y + Mathf.Sin(edge_left_cross_left_dir) * lightSize * 2;

						Vector2D intersectionLeft = Math2D.GetPointLineIntersectLine(new Pair2D(new Vector2D(edge_intersection), new Vector2D(edge_left_cross2)), new Pair2D(new Vector2D(core_left_270), new Vector2D(core_left_270_edge)));
						if (intersectionLeft != null) {

							edge_left_cross2.x = (float)intersectionLeft.x;
							edge_left_cross2.y = (float)intersectionLeft.y;

							//Draw(new Vector2D(edge_intersection), new Vector2D(edge_left_cross_left), 8 );

							// First Fin Left
							GL.TexCoord3(penumbraRect.x, penumbraRect.y, 0);
							GL.Vertex3(edge_world_left.x, edge_world_left.y, z);
						
							GL.TexCoord3(penumbraRect.width, penumbraRect.y + penumbraRect.height, 0);
							GL.Vertex3(edge_left_cross2.x, edge_left_cross2.y, z);

							GL.TexCoord3(penumbraRect.x, penumbraRect.height, 0);
							GL.Vertex3(edge_intersection.x, edge_intersection.y, z);

							// Second To Last Fin Right
							GL.TexCoord3(penumbraRect.width, penumbraRect.y+ penumbraRect.height  + penumbraSize.y / 2, 0);
							GL.Vertex3(core_right_90_edge.x, core_right_90_edge.y, z);

							GL.TexCoord3(penumbraRect.width, penumbraRect.y + penumbraRect.height, 0);
							GL.Vertex3(edge_left_cross2.x, edge_left_cross2.y, z);

							GL.TexCoord3(penumbraRect.x, penumbraRect.y + penumbraSize.y / 2, 0);	
							GL.Vertex3(edge_intersection.x, edge_intersection.y, z);

							
							// Last Fine Right
						
							GL.TexCoord3(penumbraRect.x+ penumbraSize.x / 2, penumbraRect.height + penumbraSize.y / 2, 0);	
							GL.Vertex3(core_right_90_edge.x, core_right_90_edge.y, z);

							GL.TexCoord3(penumbraRect.x+ penumbraSize.x / 2, penumbraRect.y + penumbraSize.y / 2, 0);	
							GL.Vertex3(edge_left_cross2.x, edge_left_cross2.y, z);
							
							GL.TexCoord3(penumbraRect.width + penumbraSize.x / 2, penumbraRect.y+ penumbraRect.height  + penumbraSize.y / 2, 0);
							GL.Vertex3(core_left_270_edge.x, core_left_270_edge.y, z);

						}

						GL.Color(Color.black);
						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(edge_world_left.x, edge_world_left.y, z);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(edge_world_right.x, edge_world_right.y, z);
						
					
						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(edge_intersection.x, edge_intersection.y, z);

						GL.Color(Color.white);
						
					} else {
						GL.TexCoord3(penumbraRect.x, penumbraRect.y, 0);
						GL.Vertex3(edge_world_left.x, edge_world_left.y, z);
					
						GL.TexCoord3(penumbraRect.width, penumbraRect.y+ penumbraRect.height, 0);
						GL.Vertex3(core_left_270_edge.x, core_left_270_edge.y, z);

						GL.TexCoord3(penumbraRect.x, penumbraRect.height, 0);
						GL.Vertex3(core_left_90_edge.x, core_left_90_edge.y, z);

					
						GL.TexCoord3(penumbraRect.x, penumbraRect.y, 0);
						GL.Vertex3(edge_world_right.x, edge_world_right.y, z);
					
						GL.TexCoord3(penumbraRect.width, penumbraRect.y+ penumbraRect.height, 0);
						GL.Vertex3(core_right_270_edge.x, core_right_270_edge.y, z);

						GL.TexCoord3(penumbraRect.x, penumbraRect.height, 0);
						GL.Vertex3(core_right_90_edge.x, core_right_90_edge.y, z);


						GL.Color(Color.black);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(edge_world_left.x, edge_world_left.y, z);
						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(edge_world_right.x, edge_world_right.y, z);
						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(core_right_90_edge.x, core_right_90_edge.y, z);


						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(edge_world_left.x, edge_world_left.y, z);
						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(core_left_90_edge.x, core_left_90_edge.y, z);
						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(core_right_90_edge.x, core_right_90_edge.y, z);

						GL.Color(Color.white);

					}
				}
			}
		}
    }
}