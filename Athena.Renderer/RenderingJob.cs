using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;
using System.Threading;
using Athena.Core.Objects;

namespace Athena.Renderer
{
	/// <summary>
	/// 
	/// </summary>
	public class RenderingJob
	{
		/// <summary>
		/// address of job start in output picture
		/// </summary>
		public readonly IntPtr OutputOffset;

		/// <summary>
		/// start position in output picture
		/// </summary>
		public readonly Vector2i OffsetPosition;

		/// <summary>
		/// job position in output picture
		/// </summary>
		public readonly Vector2i Position;

		/// <summary>
		/// size of job (number of pixels/rays in each direction)
		/// </summary>
		public readonly Vector2i Size;

		/// <summary>
		/// iterator for new line in output picture
		/// </summary>
		public readonly int NextOutputLineOffset;

		public readonly Camera Camera;
		public CountdownEvent DoneEvent;

		/// <summary>
		/// debug color tone for this job
		/// </summary>
		public ColorRGB DebugColor { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public ColorRGB[] ColorBuffer { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="outputOffset"></param>
		/// <param name="offsetPosition"></param>
		/// <param name="position"></param>
		/// <param name="size"></param>
		/// <param name="nextOutputLineOffset"></param>
		/// <param name="camera"></param>
		/// <param name="doneEvent"></param>
		public RenderingJob(IntPtr outputOffset, Vector2i offsetPosition, Vector2i position, Vector2i size, int nextOutputLineOffset, Camera camera, CountdownEvent doneEvent)
		{
			OutputOffset = outputOffset;
			OffsetPosition = offsetPosition;
			Position = position;
			Size = size;
			NextOutputLineOffset = nextOutputLineOffset;
			Camera = camera;
			DoneEvent = doneEvent;

			DebugColor = new ColorRGB(position.X % 2 == 0 ? (position.Y % 2 == 0 ? .5f : 1f) : (position.Y % 2 == 0 ? 1f : .5f));
			ColorBuffer = new ColorRGB[size.X * size.Y];
		}
	}
}
