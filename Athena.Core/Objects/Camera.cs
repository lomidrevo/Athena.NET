using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;
using Athena.Core.Math;
using System.Xml.Serialization;

namespace Athena.Core.Objects
{
	public class Camera
	{
		public Vector3 Position { get; set; }
		public Vector3 Target { get; set; }

		public Vector3 MovementSpeed { get; set; }
		public Vector3 RotationSpeed { get; set; }

		public Vector3 PixelWidthIterator { get; private set; }

		public Vector3 PixelHeightIterator { get; private set; }

		public Vector3 ImageOrigin { get; private set; }

		public Vector3 ViewDirection { get; private set; }

		public float FieldOfVision { get; set; }

		#region private variables

		private object updateSync = new Object();
		private bool updated = false;

		#endregion

		/// <summary>
		/// 
		/// </summary>
		public Camera()
		{
			Position = new Vector3(0f, 0f, -500f);
			Target = new Vector3(0f);

			FieldOfVision = 45f;
			
			MovementSpeed = new Vector3(1);
			RotationSpeed = new Vector3(.1f);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="target"></param>
		public Camera(Vector3 position, Vector3 target) : this()
		{
			Position = position;
			Target = target;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="outputSize"></param>
		public void Update(Vector2i outputSize)
		{
			if (updated)
			    return;

			lock (updateSync)
			{
				ViewDirection = (Target - Position).Normalize();

				#region compute view corners

				var aspectRatio = (float)outputSize.X / (float)outputSize.Y;
				var targetDistance = 1000f;

				var sizex = (float)System.Math.Tan((FieldOfVision * .5f) * ((float)System.Math.PI / 180f)) * targetDistance;
				var sizey = sizex / aspectRatio;

				var viewCorners1 = new Vector3(-sizex, sizey, targetDistance);
				var viewCorners2 = new Vector3(sizex, sizey, targetDistance);
				var viewCorners3 = new Vector3(sizex, -sizey, targetDistance);
				var viewCorners4 = new Vector3(-sizex, -sizey, targetDistance);

				var zaxis = (Target - Position).Normalize();
				var xaxis = Vector3.Cross(Vector3.UnitY, zaxis);
				var yaxis = Vector3.Cross(xaxis, -zaxis);

				var m = new Matrix4x4();
				m.Box[0] = xaxis.X; m.Box[1] = xaxis.Y; m.Box[2] = xaxis.Z;
				m.Box[4] = yaxis.X; m.Box[5] = yaxis.Y; m.Box[6] = yaxis.Z;
				m.Box[8] = zaxis.X; m.Box[9] = zaxis.Y; m.Box[10] = zaxis.Z;
				m.Invert();

				m.Box[3] = Position.X;
				m.Box[7] = Position.Y;
				m.Box[11] = Position.Z;

				Position = m.Transform(new Vector3(0f));
				viewCorners1 = m.Transform(viewCorners1);
				viewCorners2 = m.Transform(viewCorners2);
				viewCorners3 = m.Transform(viewCorners3);
				viewCorners4 = m.Transform(viewCorners4);

				#endregion

				PixelWidthIterator = (viewCorners3 - viewCorners4) / (float)outputSize.X;
				PixelHeightIterator = (viewCorners1 - viewCorners4) / (float)outputSize.Y;

				ImageOrigin = viewCorners4 + PixelWidthIterator * .5f + PixelHeightIterator * .5f;

				updated = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rotationVector"></param>
		public void RotateAroundTarget(Vector3 rotationVector)
		{
			lock (updateSync)
			{
				rotationVector *= RotationSpeed;

				if (rotationVector.X != 0)
					Position = Position.RotateAround(Target, Vector3.UnitX, rotationVector.X * (float)System.Math.PI / 180f);

				if (rotationVector.Y != 0)
					Position = Position.RotateAround(Target, Vector3.UnitY, rotationVector.Y * (float)System.Math.PI / 180f);

				updated = false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="moveVector"></param>
		public void Move(Vector3 moveVector)
		{
			lock (updateSync)
			{
				Position += moveVector * MovementSpeed;

				updated = false;
			}
		}
	}
}
