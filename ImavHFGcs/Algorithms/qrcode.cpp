// This is the main DLL file.

#include "stdafx.h"

#include "QRCode.h"
#include <zbar.h>
#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

using namespace HighFlyers::Algorithms;
using namespace System;

void QRCode::BmpToGrayRaw(unsigned char* input, int width, int height, unsigned char* output)
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

String^ QRCode::RecognizeQrCode(unsigned char* img, int width, int height)
{
	using namespace zbar;
	
	unsigned char* imgGray = new unsigned char[width*height];
	BmpToGrayRaw(img, width, height, imgGray);
	
	ImageScanner scanner;
	scanner.set_config(ZBAR_NONE, ZBAR_CFG_ENABLE, 0);
	scanner.set_config(ZBAR_QRCODE, ZBAR_CFG_ENABLE, 1);
		
	cv::Mat capture(height, width, CV_8U, imgGray), capture2(height, width, CV_8U);
	//cv::imshow("gray",capture);
	cv::medianBlur(capture, capture, 3);
	cv::GaussianBlur(capture, capture, cv::Size(3, 3), 1);
	//cv::imshow("CAPTURE2",capture);
	unsigned char* raw = (unsigned char*)(capture.data);
	Image image(width, height, "Y800", raw, width * height);
	scanner.scan(image);
	for(SymbolIterator symbol = image.symbol_begin(); symbol != image.symbol_end(); symbol)
	 {
		String^ str = gcnew String(symbol->get_data().c_str());
	
		image.set_data(NULL, 0);
		delete [] imgGray;
		return str;
    }
	
	image.set_data(NULL, 0);
	delete [] imgGray;
	return nullptr;
}