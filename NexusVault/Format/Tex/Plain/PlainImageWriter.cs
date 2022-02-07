/*******************************************************************************
 * Copyright (C) 2018-2022 MarbleBag
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>
 *
 * SPDX-License-Identifier: AGPL-3.0-or-later
 *******************************************************************************/

using NexusVault.Format.Tex;
using NexusVault.Format.Tex.Util;
using System;

public static class PlainImageWriter {

	public static byte[] GetBinary(TextureType target, Image image) {
		switch (target) {
			case TextureType.ARGB1:
			case TextureType.ARGB2: {
				switch (image.Format) {
					case ImageFormat.ARGB:
						return ColorModelConverter.ConvertARGBToBGRA(image.Data);
					case ImageFormat.RGB:
						return ColorModelConverter.ConvertRGBToBGRA(image.Data);
					case ImageFormat.Grayscale:
						return ColorModelConverter.ConvertGrayscaleToARGB(image.Data);
				}
				break;
			}
			case TextureType.Grayscale: {
				switch (image.Format) {
					case ImageFormat.ARGB:
						return ColorModelConverter.ConvertARGBToGrayscale(image.Data);
					case ImageFormat.RGB:
						return ColorModelConverter.ConvertRGBToGrayscale(image.Data);
					case ImageFormat.Grayscale:
                            return image.Data;
				}
				break;
			}
			case TextureType.RGB: {
				switch (image.Format) {
					case ImageFormat.ARGB:
						return ColorModelConverter.PackARGBToBGR565(image.Data);
					case ImageFormat.RGB:
						return ColorModelConverter.PackRGBToBGR565(image.Data);
					case ImageFormat.Grayscale:
						return ColorModelConverter.PackGrayscaleToBGR565(image.Data);
				}
				break;
			}
			default:
				throw new ArgumentException($"Invalid texture type: {target}.");
		}

		throw new ArgumentException($"Unable to write image with format {image.Format} as {target}.");
	}
}
