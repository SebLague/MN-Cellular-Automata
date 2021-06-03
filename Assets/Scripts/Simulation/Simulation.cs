using UnityEngine;
using UnityEngine.Experimental.Rendering;
using ComputeShaderUtility;

public class Simulation : SimulationBase
{

	const int simKernel = 0;
	const int displayKernel = 1;

	public int width = 1920;
	int height = 1080;

	public ComputeShader simCompute;
	public FilterMode filterMode = FilterMode.Point;

	[Header("Info")]
	public int frameCounter;

	RenderTexture simulationMap;
	RenderTexture nextSimulationMap;
	RenderTexture displayTexture;

	ComputeBuffer ruleBuffer;

	public void Reset()
	{
		frameCounter = 0;
	}

	protected override void Init()
	{
		height = Mathf.RoundToInt(width * 9 / 16f);
		const int targetFPS = 60;
		Time.fixedDeltaTime = 1.0f / targetFPS;

		GraphicsFormat displayFormat = GraphicsFormat.R32G32B32A32_SFloat;
	
		ComputeHelper.CreateRenderTexture(ref simulationMap, width, height, filterMode, displayFormat, "Sim Texture");
		ComputeHelper.CreateRenderTexture(ref nextSimulationMap, width, height, filterMode, displayFormat, "Next Sim Texture");
		ComputeHelper.CreateRenderTexture(ref displayTexture, width, height, filterMode, displayFormat, "Display Texture");

		ComputeHelper.SetRenderTexture(simulationMap, simCompute, "SimMap", simKernel, displayKernel);
		ComputeHelper.SetRenderTexture(nextSimulationMap, simCompute, "NextSimMap", simKernel);
		ComputeHelper.SetRenderTexture(displayTexture, simCompute, "DisplayMap", displayKernel);

		simCompute.SetInt("width", width);
		simCompute.SetInt("height", height);
		simCompute.SetVector("noiseOffset", settings.noiseOffset);

		material.mainTexture = displayTexture;
		frameCounter = 0;
	}


	protected override void RunSimulation()
	{
		simCompute.SetInt("frameCount", frameCounter);

		ComputeHelper.CreateStructuredBuffer<Rule>(ref ruleBuffer, settings.rules);
		simCompute.SetBuffer(simKernel, "Rules", ruleBuffer);

		// Run
		ComputeHelper.Dispatch(simCompute, width, height, 1, simKernel);

		ComputeHelper.CopyRenderTexture(nextSimulationMap, simulationMap);

		frameCounter++;
	}

	protected override void UpdateDisplay()
	{
		ComputeHelper.Dispatch(simCompute, width, height, 1, displayKernel);
	}

	protected override void HandleInput()
	{

	}

	protected override void ReleaseBuffers()
	{
		ComputeHelper.Release(ruleBuffer);
		ComputeHelper.Release(simulationMap, nextSimulationMap, displayTexture);
	}


}
