/*
This file is part of Extraplanetary Launchpads.

Extraplanetary Launchpads is free software: you can redistribute it and/or
modify it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Extraplanetary Launchpads is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Extraplanetary Launchpads.  If not, see
<http://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

using KodeUI;

namespace ExtraplanetaryLaunchpads {

	public class ELCraftThumbManager : UIImage
	{
		public class ThumbSprite
		{
			Sprite _sprite;
			public Sprite sprite
			{
				get { return _sprite; }
				set {
					_sprite = value;
					onUpdate.Invoke (this);
				}
			}

			public class ThumbEvent : UnityEvent<ThumbSprite> { }
			public ThumbEvent onUpdate { get; private set; }

			public ThumbSprite (Sprite sprite)
			{
				_sprite = sprite;
				onUpdate = new ThumbEvent ();
			}
		}

		static Texture2D genericCraftThumb;

		static Dictionary<string, ThumbSprite> thumbnailCache = new Dictionary<string, ThumbSprite> ();
		public static ThumbSprite GetThumb (string thumbPath)
		{
			// FIXME use new icon :)
			if (!genericCraftThumb) {
				genericCraftThumb = AssetBase.GetTexture("craftThumbGeneric");
			}

			ThumbSprite thumb;
			if (thumbnailCache.TryGetValue (thumbPath, out thumb) && thumb.sprite ) {
				return thumb;
			}

			var thumbTex = GameObject.Instantiate (genericCraftThumb) as Texture2D;
			bool ok = EL_Utils.LoadImage (ref thumbTex, thumbPath);
			Debug.Log ($"[ELCraftThumbManager] GetThumb {thumbPath} {ok}");
			var sprite = EL_Utils.MakeSprite (thumbTex);

			if (thumb == null) {
				thumb = new ThumbSprite (sprite);
			} else {
				thumb.sprite = sprite;
			}

			thumbnailCache[thumbPath] = thumb;
			return thumb;
		}

		public static bool UpdateThumbCache (string thumbPath, Texture2D tex)
		{
			ThumbSprite thumb;
			tex.Apply ();
			Debug.Log ($"[ELCraftThumbManager] UpdateThumbCache {thumbPath} {tex}");
			if (thumbnailCache.TryGetValue (thumbPath, out thumb)) {
				Debug.Log ($"    update");
				GameObject.Destroy (thumb.sprite.texture);
				GameObject.Destroy (thumb.sprite);
				thumb.sprite = EL_Utils.MakeSprite (tex);
				return true;
			}
			Debug.Log ($"    new");
			return false;
		}
	}
}