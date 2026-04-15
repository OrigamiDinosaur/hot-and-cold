using UnityEngine;

namespace Apache.Core.Extensions {
	public static class RectExtensions {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		// Setters.

		/// <summary>Sets the size of the rect (anchored top left).</summary>
		public static Rect WithSize(this Rect self, Vector2 size) {
			self.size = size;
			return self;
		}

		/// <summary>Sets the width of the rect (anchored left).</summary>
		public static Rect WithWidth(this Rect self, float width) {
			self.width = width;
			return self;
		}

		/// <summary>Sets the height of the rect (anchored top).</summary>
		public static Rect WithHeight(this Rect self, float height) {
			self.height = height;
			return self;
		}

		/// <summary>Sets the position of the rect (anchored top left).</summary>
		public static Rect WithPosition(this Rect self, Vector2 position) {
			self.position = position;
			return self;
		}

		/// <summary>Sets the x position of the rect (anchored left).</summary>
		public static Rect WithX(this Rect self, float x) {
			self.x = x;
			return self;
		}

		/// <summary>Sets the y position of the rect (anchored top).</summary>
		public static Rect WithY(this Rect self, float y) {
			self.y = y;
			return self;
		}

		// Offsetting.

		/// <summary>Offsets the rect by 'offset'.</summary>
		public static Rect OffsetPosition(this Rect self, Vector2 offset) {
			self.position += offset;
			return self;
		}

		/// <summary>Offsets the x position of the rect by 'offset' (anchored left).</summary>
		public static Rect OffsetX(this Rect self, float offset) {
			self.x += offset;
			return self;
		}

		/// <summary>Offsets the y position of the rect by 'offset' (anchored top).</summary>
		public static Rect OffsetY(this Rect self, float offset) {
			self.y += offset;
			return self;
		}

		// Padding.

		/// <summary>Offset the size of the rect (anchored center).</summary>
		public static Rect Pad(this Rect self, float padding) {
			self.position += new Vector2(padding, padding);
			self.size -= new Vector2(padding, padding) * 2f;
			return self;
		}

		/// <summary>Offset the height of the rect (anchored bottom).</summary>
		public static Rect PadTop(this Rect self, float padding) {
			self.y -= padding;
			self.height += padding;
			return self;
		}

		/// <summary>Offset the width of the rect (anchored left).</summary>
		public static Rect PadRight(this Rect self, float padding) {
			self.width += padding;
			return self;
		}

		/// <summary>Offset the height of the rect (anchored top).</summary>
		public static Rect PadBottom(this Rect self, float padding) {
			self.height += padding;
			return self;
		}

		/// <summary>Offset the width of the rect (anchored right).</summary>
		public static Rect PadLeft(this Rect self, float padding) {
			self.x -= padding;
			self.width += padding;
			return self;
		}

		// Clipping.

		/// <summary>Clip (inverse offset) the size of the rect (anchored center).</summary>
		public static Rect Clip(this Rect self, float padding) {
			self.position -= new Vector2(padding, padding);
			self.size += new Vector2(padding, padding) * 2f;
			return self;
		}

		/// <summary>Clip (inverse offset) the height of the rect (anchored bottom).</summary>
		public static Rect ClipTop(this Rect self, float padding) {
			self.y += padding;
			self.height -= padding;
			return self;
		}

		/// <summary>Clip (inverse offset) the width of the rect (anchored left).</summary>
		public static Rect ClipRight(this Rect self, float padding) {
			self.width -= padding;
			return self;
		}

		/// <summary>Clip (inverse offset) the height of the rect (anchored top).</summary>
		public static Rect ClipBottom(this Rect self, float padding) {
			self.height -= padding;
			return self;
		}

		/// <summary>Clip (inverse offset) the width of the rect (anchored right).</summary>
		public static Rect ClipLeft(this Rect self, float padding) {
			self.x += padding;
			self.width -= padding;
			return self;
		}

		// Trimming. 

		/// <summary>Set the height of the rect (anchored bottom).</summary>
		public static Rect TrimTop(this Rect self, float height) {
			self.y = self.y + self.height - height;
			self.height = height;
			return self;
		}

		/// <summary>Set the width of the rect (anchored left).</summary>
		public static Rect TrimRight(this Rect self, float width) {
			self.width = width;
			return self;
		}

		/// <summary>Set the height of the rect (anchored top).</summary>
		public static Rect TrimBottom(this Rect self, float height) {
			self.height = height;
			return self;
		}

		/// <summary>Set the width of the rect (anchored right).</summary>
		public static Rect TrimLeft(this Rect self, float width) {
			self.x = self.x + self.width - width;
			self.width = width;
			return self;
		}

		// Splitting.

		/// <summary>Split the rect into 'count' columns and return the column rect for column[index].</summary>
		public static Rect SplitX(this Rect self, int index, int count) {
			int width = (int)(self.width / count);
			self.width = width;
			self.x += width * index;
			return self;
		}

		/// <summary>Split the rect into 'count' rows and return the row rect for row[index].</summary>
		public static Rect SplitY(this Rect self, int index, int count) {
			int height = (int)(self.height / count);
			self.height = height;
			self.y += height * index;
			return self;
		}

		// Zeroing.

		/// <summary>Sets the rect's position to zero.s</summary>
		public static Rect ZeroPosition(this Rect self) {
			self.position = Vector2.zero;
			return self;
		}

		/// <summary>Sets the rect's size to zero.s</summary>
		public static Rect ZeroSize(this Rect self) {
			self.size = Vector2.zero;
			return self;
		}
	}
}