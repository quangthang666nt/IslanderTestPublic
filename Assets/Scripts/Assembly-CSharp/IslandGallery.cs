using System;
using FlatBuffers;
using Islanders;

[Serializable]
public class IslandGallery
{
	public byte[] highres = new byte[0];

	public byte[] lowres = new byte[0];

	public Offset<Islanders.IslandGallery> ToFlatBuffer(FlatBufferBuilder builder)
	{
		VectorOffset highresOffset = Islanders.IslandGallery.CreateHighresVector(builder, highres);
		VectorOffset lowresOffset = Islanders.IslandGallery.CreateLowresVector(builder, lowres);
		Islanders.IslandGallery.StartIslandGallery(builder);
		Islanders.IslandGallery.AddHighres(builder, highresOffset);
		Islanders.IslandGallery.AddLowres(builder, lowresOffset);
		return Islanders.IslandGallery.EndIslandGallery(builder);
	}

	public void FromFlatBuffer(Islanders.IslandGallery islandGalleryBuffer)
	{
		if (islandGalleryBuffer.HighresLength > 0)
		{
			highres = islandGalleryBuffer.GetHighresArray();
		}
		if (islandGalleryBuffer.LowresLength > 0)
		{
			lowres = islandGalleryBuffer.GetLowresArray();
		}
	}
}
