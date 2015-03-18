#include "stdafx.h"

#include "DummyMission.h"
#include <opencv2/core/core.hpp>
//#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

using namespace HighFlyers::Algorithms;
using namespace System;

void DummyMission::BmpToGrayRaw(unsigned char* input, int width, int height, unsigned char* output)
{
	int j = width * height;
	int stride = width * 3;
	
	for (int i = 0; i < height; i++)
	{
		for (int j = 0; j < width; j++)
		{
			int curr = i * stride + j * 3 + 54;
			int sum = input[curr] + input[curr + 1] + input[curr + 2];
			int outputPos = width * (i + 1) - 1 - j;
			output[outputPos] = sum / 3;
			input[curr] = output[outputPos];
			input[curr+1] = output[outputPos];
			input[curr+2] = output[outputPos];	
		}
	}
}

unsigned char* DummyMission::DoMission(unsigned char* img, int width, int height)
{
	unsigned char* imgGray = new unsigned char[width*height];
	BmpToGrayRaw(img, width, height, imgGray);

	cv::Mat capture, capture2;
	capture.data = imgGray;
	capture.cols = width;
	capture.rows = height;
	cv::equalizeHist(capture, capture2);
	

	unsigned char* raw = (unsigned char*)(capture2.data);
	return raw;
}