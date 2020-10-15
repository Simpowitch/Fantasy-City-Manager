using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class Main : Base {

		public static Pair2D pair = Pair2D.Zero();

		public static void Draw(LightingBuffer2D buffer, List<Polygon2D> polygons, float lightSizeSquared, float z, Vector2 offset, Vector2 scale, float height) {
			if (Lighting2D.commonSettings.highQualityShadows == LightingSettings.QualitySettings.ShadowQuality.Soft) {
				DrawSoft(buffer, polygons, lightSizeSquared, z, offset, scale);
			} else {
				DrawLegacy(buffer, polygons, lightSizeSquared, z, offset, scale, height);
			}
		}

		public static void DrawLegacy(LightingBuffer2D buffer, List<Polygon2D> polygons,  float lightSizeSquared, float z, Vector2 offset, Vector2 scale, float height) {
			Vector2 vA, pA, vB, pB, vC, vD, mA;
			float angleA, angleB, rot;

			int PolygonCount = polygons.Count;

			float outerAngle = buffer.lightSource.outerAngle;
			bool drawInside = false;
			bool culling = true;

			if (height > 0) {
				lightSizeSquared = height;
				outerAngle = 0;
				culling = false;
			}

			for(int i = 0; i < PolygonCount; i++) {

				if (ShadowSetup.drawAbove == false && polygons[i].PointInPoly (-offset)) {
					drawInside = true;
				} else {
					drawInside = false;
				}

				List<Vector2D> pointsList = polygons[i].pointsList;
				int pointsCount = pointsList.Count;
			
				 for(int x = 0; x < pointsCount; x++) {
					int next = (x + 1) % pointsCount;
					
					pair.A = pointsList[x];
                    pair.B = pointsList[next];

					float p_a_x = (float)pair.A.x * scale.x;
					float p_a_y = (float)pair.A.y * scale.y;

					float p_b_x = (float)pair.B.x * scale.x;
					float p_b_y = (float)pair.B.y * scale.y;

					float pa_x = p_a_x + offset.x;
					float pa_y = p_a_y + offset.y;

					float pb_x = p_b_x + offset.x;
					float pb_y = p_b_y + offset.y;
					
					float lightDirection = Mathf.Atan2((pa_y + pb_y) / 2 , (pa_x + pb_x) / 2 ) * Mathf.Rad2Deg;
					float EdgeDirection = Mathf.Atan2(p_a_y - p_b_y, p_a_x - p_b_x) * Mathf.Rad2Deg - 180;

					lightDirection -= EdgeDirection;
	
					lightDirection = (lightDirection + 720) % 360;
					
					if (culling) {
						if (drawInside) {
							if (lightDirection > 180) {
								continue;
							}
						} else {
							if (lightDirection < 180) {
								continue;
							}
						}
					}
					
					vA.x = pa_x;
					vA.y = pa_y;

					pA.x = pa_x;
					pA.y = pa_y;

					vB.x = pb_x;
					vB.y = pb_y;

					pB.x = pb_x;
					pB.y = pb_y;

					vC.x = pa_x;
					vC.y = pa_y;

					vD.x = pb_x;
					vD.y = pb_y;

					angleA = (float)System.Math.Atan2 (vA.y, vA.x);
					angleB = (float)System.Math.Atan2 (vB.y, vB.x);

					vA.x += Mathf.Cos(angleA) * lightSizeSquared;
					vA.y += Mathf.Sin(angleA) * lightSizeSquared;

					vB.x += Mathf.Cos(angleB) * lightSizeSquared;
					vB.y += Mathf.Sin(angleB) * lightSizeSquared;

					rot = angleA - Mathf.Deg2Rad * buffer.lightSource.outerAngle;

					pA.x += Mathf.Cos(rot) * lightSizeSquared;
					pA.y += Mathf.Sin(rot) * lightSizeSquared;

					rot = angleB + Mathf.Deg2Rad * buffer.lightSource.outerAngle;

					pB.x += Mathf.Cos(rot) * lightSizeSquared;
					pB.y += Mathf.Sin(rot) * lightSizeSquared;

					if (outerAngle > 0) {
						GL.Color(Color.white);

						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y, 0);
						GL.Vertex3(vC.x, vC.y, z);

						GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y, 0);
						GL.Vertex3(pA.x, pA.y, z);
						
						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.height, 0);
						GL.Vertex3(vA.x, vA.y, z);
						
						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y, 0);
						GL.Vertex3(vD.x, vD.y, z);

						GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y, 0);
						GL.Vertex3(pB.x, pB.y, z);
						
						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.height, 0);
						GL.Vertex3(vB.x, vB.y, z);
					}

					GL.Color(Color.black);

					if (FillWhite.highQuality) {
						// Detailed Shadow
						mA.x = (vC.x + vD.x) / 2;
						mA.y = (vC.y + vD.y) / 2;

						rot = (float)System.Math.Atan2 (mA.y, mA.x);

						mA.x += Mathf.Cos(rot) * lightSizeSquared;
						mA.y += Mathf.Sin(rot) * lightSizeSquared;

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vC.x, vC.y, z);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vD.x, vD.y, z);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(mA.x, mA.y, z);

						// Regular Shadow
						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vC.x, vC.y, z);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(mA.x, mA.y, z);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vA.x, vA.y, z);
						
						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(mA.x, mA.y, z);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vB.x, vB.y, z);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vD.x, vD.y, z);
						
					} else {

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vA.x, vA.y, z);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vB.x, vB.y, z);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vC.x, vC.y, z);


						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vB.x, vB.y, z);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vD.x, vD.y, z);

						GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
						GL.Vertex3(vC.x, vC.y, z);
					}
				}
			}
		}

		public static void DrawSoft(LightingBuffer2D buffer, List<Polygon2D> polygons, float lightSizeSquared, float z, Vector2 offset, Vector2 scale) {
			float lightSize = buffer.lightSource.size;
			float lightCoreSize = buffer.lightSource.coreSize;

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

				if (ShadowSetup.drawAbove && polygons[i].PointInPoly (-offset)) {
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
						GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y+ Penumbra.uvRect.height  + Penumbra.size.y / 2, 0);
						GL.Vertex3(core_left_90_edge.x, core_left_90_edge.y, z);

						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.height + Penumbra.size.y / 2, 0);
						GL.Vertex3(mid_x, mid_y, z);

						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y + Penumbra.size.y / 2, 0);				
						GL.Vertex3(edge_intersection.x, edge_intersection.y, z);


						GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y+ Penumbra.uvRect.height  + Penumbra.size.y / 2, 0);
						GL.Vertex3(core_right_90_edge.x, core_right_90_edge.y, z);

						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.height + Penumbra.size.y / 2, 0);
						GL.Vertex3(mid_x, mid_y, z);

						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y + Penumbra.size.y / 2, 0);				
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
							GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y, 0);
							GL.Vertex3(edge_world_right.x, edge_world_right.y, z);
						
							GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y + Penumbra.uvRect.height, 0);
							GL.Vertex3(edge_right_cross2.x, edge_right_cross2.y, z);

							GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.height, 0);
							GL.Vertex3(edge_intersection.x, edge_intersection.y, z);

							// Second To Last Fin Right
							GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y+ Penumbra.uvRect.height  + Penumbra.size.y / 2, 0);
							GL.Vertex3(core_left_90_edge.x, core_left_90_edge.y, z);

							GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y + Penumbra.uvRect.height, 0);
							GL.Vertex3(edge_right_cross2.x, edge_right_cross2.y, z);

							GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y + Penumbra.size.y / 2, 0);	
							GL.Vertex3(edge_intersection.x, edge_intersection.y, z);


							// Last Fine Right
							GL.TexCoord3(Penumbra.uvRect.x+ Penumbra.size.x / 2, Penumbra.uvRect.height + Penumbra.size.y / 2, 0);
							GL.Vertex3(core_left_90_edge.x, core_left_90_edge.y, z);
							
							GL.TexCoord3(Penumbra.uvRect.x+ Penumbra.size.x / 2, Penumbra.uvRect.y + Penumbra.size.y / 2, 0);	
							GL.Vertex3(edge_right_cross2.x, edge_right_cross2.y, z);

							GL.TexCoord3(Penumbra.uvRect.width+ Penumbra.size.x / 2, Penumbra.uvRect.y+ Penumbra.uvRect.height  + Penumbra.size.y / 2, 0);
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
							GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y, 0);
							GL.Vertex3(edge_world_left.x, edge_world_left.y, z);
						
							GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y + Penumbra.uvRect.height, 0);
							GL.Vertex3(edge_left_cross2.x, edge_left_cross2.y, z);

							GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.height, 0);
							GL.Vertex3(edge_intersection.x, edge_intersection.y, z);

							// Second To Last Fin Right
							GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y+ Penumbra.uvRect.height  + Penumbra.size.y / 2, 0);
							GL.Vertex3(core_right_90_edge.x, core_right_90_edge.y, z);

							GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y + Penumbra.uvRect.height, 0);
							GL.Vertex3(edge_left_cross2.x, edge_left_cross2.y, z);

							GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y + Penumbra.size.y / 2, 0);	
							GL.Vertex3(edge_intersection.x, edge_intersection.y, z);

							
							// Last Fine Right
						
							GL.TexCoord3(Penumbra.uvRect.x+ Penumbra.size.x / 2, Penumbra.uvRect.height + Penumbra.size.y / 2, 0);	
							GL.Vertex3(core_right_90_edge.x, core_right_90_edge.y, z);

							GL.TexCoord3(Penumbra.uvRect.x+ Penumbra.size.x / 2, Penumbra.uvRect.y + Penumbra.size.y / 2, 0);	
							GL.Vertex3(edge_left_cross2.x, edge_left_cross2.y, z);
							
							GL.TexCoord3(Penumbra.uvRect.width + Penumbra.size.x / 2, Penumbra.uvRect.y+ Penumbra.uvRect.height  + Penumbra.size.y / 2, 0);
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
						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y, 0);
						GL.Vertex3(edge_world_left.x, edge_world_left.y, z);
					
						GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y+ Penumbra.uvRect.height, 0);
						GL.Vertex3(core_left_270_edge.x, core_left_270_edge.y, z);

						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.height, 0);
						GL.Vertex3(core_left_90_edge.x, core_left_90_edge.y, z);

					
						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y, 0);
						GL.Vertex3(edge_world_right.x, edge_world_right.y, z);
					
						GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y+ Penumbra.uvRect.height, 0);
						GL.Vertex3(core_right_270_edge.x, core_right_270_edge.y, z);

						GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.height, 0);
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
