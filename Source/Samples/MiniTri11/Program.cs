﻿// Copyright (c) 2007-2011 SlimDX Group
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SlimDX.DXGI;
using SlimDX;
using SlimDX.Direct3D11;

namespace MiniTri11
{
	static class Program
	{
		static void Main()
		{
			ReferenceTracker.TrackReferences = true;
			Form form = new Form();

			IDXGIFactory factory = DXGI.CreateFactory();
			IDXGIAdapter adapter = null;
			factory.EnumAdapters(0, out adapter);

			DXGI_SWAP_CHAIN_DESC swapChainDescription = new DXGI_SWAP_CHAIN_DESC
			{
				BufferCount = 1,
				BufferDesc = new DXGI_MODE_DESC
				{
					Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
					Height = 100,
					RefreshRate = new DXGI_RATIONAL
					{
						Denominator = 1,
						Numerator = 60
					},

					Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_UNSPECIFIED,
					ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED,
					Width = 100
				},
				BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT,
				Flags = 0,
				OutputWindow = form.Handle,
				SampleDesc = new DXGI_SAMPLE_DESC
				{
					Count = 1,
					Quality = 0
				},
				SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD,
				Windowed = true
			};

			ID3D11Device device = Direct3D11.CreateDevice(adapter);
			IDXGISwapChain swapChain = null;
			factory.CreateSwapChain(device.NativePointer, swapChainDescription, out swapChain);

			ID3D11Texture2D backbuffer = swapChain.GetBuffer<ID3D11Texture2D>(0);
			ID3D11RenderTargetView view = null;
			device.CreateRenderTargetView(backbuffer, IntPtr.Zero, out view);

			RenderLoop loop = new RenderLoop();
			ID3D11DeviceContext context = null;
			device.GetImmediateContext(out context);
			loop.Run(form, () =>
			{
				Color4 clearColor = new Color4 { R = 0.0f, G = 0.0f, B = 1.0f, A = 1.0f };
				context.ClearRenderTargetView(view, clearColor);
				swapChain.Present(0, 0);
			});

			view.ReleaseReference();
			backbuffer.ReleaseReference();
			swapChain.ReleaseReference();
			device.ReleaseReference();
			adapter.ReleaseReference();
			factory.ReleaseReference();
		}
	}
}
