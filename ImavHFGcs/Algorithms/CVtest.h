#pragma once

namespace HighFlyers
{
namespace Algorithms 
{
	public ref class CVtest
	{
	private:
		void BmpToGrayRaw(unsigned char* input, int width, int height, unsigned char* output);
	public:
		unsigned char* Tester(unsigned char* img, int width, int height);
	};
}
}