using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class WriteWav {

	public  int bufferSize;
	public  int numBuffers;
	private  int outputRate = 44100;
	private  int headerSize = 44; //default for uncompressed wav
	private  FileStream fileStream;
	
	public  void StartWriting(String name)
	{
		fileStream = new FileStream(name, FileMode.Create);
		byte emptyByte = new byte();
		
		for (int i = 0; i < headerSize; i++) //preparing the header
		{
			fileStream.WriteByte(emptyByte);
		}
	}
	
	public  void ConvertAndWrite(float[] dataSource )
	{
		
		Int16[] intData  = new Int16[dataSource.Length];
		//converting in 2 steps : float[] to Int16[], //then Int16[] to Byte[]
		
		Byte[] bytesData   = new Byte[dataSource.Length * 2];
		//bytesData array is twice the size of
		//dataSource array because a float converted in Int16 is 2 bytes.
		
		int rescaleFactor  = 32767; //to convert float to Int16
		
		for (int i = 0; i < dataSource.Length; i++)
		{
			intData[i] = (Int16) (dataSource[i] * (float)rescaleFactor);
			Byte[] byteArr = new Byte[2];
			byteArr = BitConverter.GetBytes(intData[i]);
			byteArr.CopyTo(bytesData, i * 2);
		}
		
		fileStream.Write(bytesData, 0, bytesData.Length);
	}

	
	public  void WriteHeader()
	{
		fileStream.Seek(0, SeekOrigin.Begin);
		
		Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
		fileStream.Write(riff, 0, 4);
		
		Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
		fileStream.Write(chunkSize, 0, 4);
		
		Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
		fileStream.Write(wave, 0, 4);
		
		Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
		fileStream.Write(fmt, 0, 4);
		
		Byte[] subChunk1 = BitConverter.GetBytes(16);
		fileStream.Write(subChunk1, 0, 4);
		
		UInt16 two  = 2;
		UInt16 one = 1;
		
		Byte[] audioFormat = BitConverter.GetBytes(one);
		fileStream.Write(audioFormat, 0, 2);
		
		Byte[] numChannels = BitConverter.GetBytes(two);
		fileStream.Write(numChannels, 0, 2);
		
		Byte[] sampleRate = BitConverter.GetBytes(outputRate);
		fileStream.Write(sampleRate, 0, 4);
		
		Byte[] byteRate = BitConverter.GetBytes(outputRate * 4);
		// sampleRate * bytesPerSample*number of channels, here 44100*2*2
		
		fileStream.Write(byteRate, 0, 4);
		
		UInt16 four = 4;
		Byte[] blockAlign = BitConverter.GetBytes(four);
		fileStream.Write(blockAlign, 0, 2);
		
		UInt16 sixteen = 16;
		Byte[] bitsPerSample = BitConverter.GetBytes(sixteen);
		fileStream.Write(bitsPerSample, 0, 2);
		
		Byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
		fileStream.Write(dataString, 0, 4);
		
		Byte[] subChunk2 = BitConverter.GetBytes(fileStream.Length - headerSize);
		fileStream.Write(subChunk2, 0, 4);
		
		fileStream.Close();
	}

}
