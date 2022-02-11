using System.Threading.Tasks;
using Robotc;
using Lebai.SDK;

namespace Lebai.SDK
{
	public class Sample
	{
		public static async Task Main(string[] args)
		{
			var client = new LebaiRobotClient("192.168.3.227");
			await client.StartSys();
			var robotData = await client.GetRobotData();
			await client.GetDIO(new IOPin
			{
				Pin = 1
			});
			var claw = await client.GetClaw();
			await client.Stop();
		}
	}
}