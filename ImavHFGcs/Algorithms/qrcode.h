#pragma once

namespace HighFlyers
{
namespace Algorithms 
{
	public ref class QRCode
	{
	private:
		void BmpToGrayRaw(unsigned char* input, int width, int height, unsigned char* output);
	public:
		System::String^ RecognizeQrCode(unsigned char* img, int width, int height);
	};
}
}