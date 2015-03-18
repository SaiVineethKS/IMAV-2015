#pragma once

namespace HighFlyers
{
namespace Algorithms 
{
	public ref class DummyMission
	{
	private:
		void BmpToGrayRaw(unsigned char* input, int width, int height, unsigned char* output);
	public:
		unsigned char* DoMission(unsigned char* img, int width, int height);
	};
}
}