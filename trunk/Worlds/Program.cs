using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace AntiCulture.Worlds
{
    internal class Program
    {
        #region Data members
        private World mWorld = new World();
        private Timer mTimer = new Timer();
        private Random mRandom = new Random();
        private List<Plugin> mPlugins = new List<Plugin>();
        private List<Human> mTrackedHumans = new List<Human>();
        #endregion

        #region Constructor
        private Program()
        {
            Encyclopedia encyclopedia = new Encyclopedia();

            // Register species
            encyclopedia.Species.Add(Human.Species);

            encyclopedia.Species.Add(Entities.Steak.Species);
            encyclopedia.Species.Add(Entities.Apple.Species);
            encyclopedia.Species.Add(Entities.Water.Species);
            encyclopedia.Species.Add(Entities.Rock.Species);
            encyclopedia.Species.Add(Entities.Tree.Species);
            encyclopedia.Species.Add(Entities.Vomit.Species);
            encyclopedia.Species.Add(Entities.HealingPlant.Species);
            encyclopedia.Species.Add(Entities.Feces.Species);
            encyclopedia.Species.Add(Entities.Urine.Species);

            // Register operators
            // Note: RandomOperator is NOT registered as it currently cannot
            //       be tracked properly by entities.
            encyclopedia.Operators.Add(Operations.Go.Operator);
            encyclopedia.Operators.Add(Operations.Consume.Operator);
            encyclopedia.Operators.Add(Operations.Hit.Operator);
            encyclopedia.Operators.Add(Operations.Sleep.Operator);
            encyclopedia.Operators.Add(Operations.Vomit.Operator);
            encyclopedia.Operators.Add(Operations.Push.Operator);
            encyclopedia.Operators.Add(Operations.Defecate.Operator);
            encyclopedia.Operators.Add(Operations.Urinate.Operator);
            encyclopedia.Operators.Add(Operations.ShareKnowledge.Operator);

            // Set this encyclopedia to be used by our world
            mWorld.Encyclopedia = encyclopedia;
        }
        #endregion

        #region Methods
        private void Run()
        {
            mTimer.Reset();
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    string line = Console.ReadLine();
                    string[] parts = line.Split(new char[] { ' ' });

                    if(parts.Length > 0)
                    {
                        string command = parts[0].ToLower();
                        if (command == "quit") break;
                        List<string> arguments = new List<string>(parts);
                        arguments.RemoveAt(0);
                        ExecuteCommand(command, arguments.ToArray());
                        Console.WriteLine();
                    }

                    mTimer.PassiveTick();
                }
                else
                {
                    mTimer.Tick();
                    if (mTimer.TimeDelta > 1.0f)
                    {
                        mTimer.TimeElapsed += -mTimer.TimeDelta + 1.0f;
                        mTimer.TimeDelta = 1.0f;
                    }

                    // Update world
                    mWorld.Update(mTimer, mRandom);

                    // Update plugins
                    for (int i = 0; i < mPlugins.Count; )
                    {
                        try
                        {
                            if (mPlugins[i].Tick()) ++i;
                            else mPlugins.RemoveAt(i);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Plugin " + mPlugins[i].Name + " failed to update : " + e.Message);
                            try { mPlugins[i].Shutdown(); }
                            catch (Exception e2) { Console.WriteLine("...and to shutdown : " + e2.Message); }
                            mPlugins.RemoveAt(i);
                        }
                    }
                }

                System.Threading.Thread.Sleep(10);
            }

            // Shutdown plugins
            foreach (Plugin plugin in mPlugins)
            {
                try
                {
                    plugin.Shutdown();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Plugin " + plugin.Name + " failed to shutdown : " + e.Message);
                }
            }
            mPlugins.Clear();
        }

        private void ExecuteCommand(string command, string[] arguments)
        {
            #region add command
            if (command == "add")
            {
                if (arguments.Length != 1 && arguments.Length != 2)
                {
                    Console.WriteLine("The \"add\" command takes one or two arguments");
                }
                else
                {
                    Species species = mWorld.Encyclopedia.FindSpecies(arguments[0]);

                    if (species == null)
                    {
                        Console.WriteLine("Species \"" + arguments[0] + "\" does not exist");
                    }
                    else
                    {
                        try
                        {
                            uint count = (arguments.Length == 2) ? uint.Parse(arguments[1]) : 1;
                            for (uint i = 0; i < count; ++i)
                            {
                                Entity entity = species.Factory(mWorld);
                                entity.Position = new Vector((float)mRandom.NextDouble() * 20.0f - 10.0f, (float)mRandom.NextDouble() * 20.0f - 10.0f);
                                mWorld.Entities.Add(entity);
                            }
                            Console.WriteLine("Added " + count + " instance(s) of species \"" + arguments[0] + "\"");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Error interpreting command, was the second argument an integer?");
                        }
                    }
                }
            }
            #endregion
            #region addnamed command
            else if (command == "addnamed")
            {
                if (arguments.Length < 2)
                {
                    Console.WriteLine("The \"addnamed\" command takes at least two arguments");
                }
                else
                {
                    Species species = mWorld.Encyclopedia.FindSpecies(arguments[0]);

                    if (species == null)
                    {
                        Console.WriteLine("Species \"" + arguments[0] + "\" does not exist");
                    }
                    else
                    {
                        for (uint i = 1; i < arguments.Length; ++i)
                        {
                            Entity entity = species.Factory(mWorld);
                            entity.InstanceName = arguments[i];
                            entity.Position = new Vector((float)mRandom.NextDouble() * 20.0f - 10.0f, (float)mRandom.NextDouble() * 20.0f - 10.0f);
                            mWorld.Entities.Add(entity);
                        }
                        Console.WriteLine("Added " + (arguments.Length-1).ToString() + " instance(s) of species \"" + arguments[0] + "\"");
                    }
                }
            }
            #endregion
            #region loadspecies command
            else if (command == "loadspecies")
            {
                if (arguments.Length != 1)
                {
                    Console.WriteLine("The \"loadspecies\" command takes one argument");
                }
                else
                {
                    try
                    {
                        Species species = SimpleSpecies.FromFile("species\\" + arguments[0] + ".ssd");
                        mWorld.Encyclopedia.Species.Add(species);
                        uint count = (arguments.Length == 2) ? uint.Parse(arguments[1]) : 1;
                        Console.WriteLine("Species successfully loaded");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to load species : " + e.Message);
                    }
                }
            }
               #endregion
            #region track command
            else if (command == "track")
            {
                if (arguments.Length != 1)
                {
                    Console.WriteLine("The \"track\" command takes one argument");
                }
                else
                {
                    Human humanToTrack = mWorld.FindNamed(arguments[0]) as Human;

                    if (humanToTrack == null)
                    {
                        Console.WriteLine("Human \"" + arguments[0] + "\" not found");
                    }
                    else
                    {
                        mTrackedHumans.Add(humanToTrack);
                        humanToTrack.Dying += new EventHandler(OnHumanDie);
                        humanToTrack.OperationStarted += new OperationEventHandler(OnHumanStartOperation);
                        Console.WriteLine("Tracking human \"" + arguments[0] + "\"...");
                    }
                }
            }
            #endregion
            #region untrack command
            else if (command == "untrack")
            {
                if (arguments.Length != 1)
                {
                    Console.WriteLine("The \"untrack\" command takes one argument");
                }
                else
                {
                    for (int i = 0; i < mTrackedHumans.Count; ++i)
                    {
                        if (mTrackedHumans[i].InstanceName == arguments[0])
                        {
                            mTrackedHumans.RemoveAt(i);
                            Console.WriteLine("Human \"" + arguments[0] + "\" is not tracked anymore");
                            break;
                        }
                        else if (i == mTrackedHumans.Count - 1)
                        {
                            Console.WriteLine("Human \"" + arguments[0] + "\" not found in tracking list");
                        }
                    }
                }
            }
            #endregion
            #region stat command
            else if (command == "stat")
            {
                uint entityCount = 0;
                foreach (Entity i in mWorld.Entities)
                    ++entityCount;
                Console.WriteLine("There are " + entityCount.ToString() + " entities in the world");
            }
            #endregion
            #region plug command
            else if (command == "plug")
            {
                if (arguments.Length != 1)
                {
                    Console.WriteLine("The \"plug\" command takes one argument");
                }
                else
                {
                    try
                    {
                        string path = Assembly.GetExecutingAssembly().Location;
                        path = path.Substring(0, path.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
                        path += "plugins\\";
                        Assembly assembly = Assembly.LoadFile(path + arguments[0] + ".dll");
                        Plugin plugin = (Plugin)assembly.CreateInstance("AntiCulture.Worlds.Plugins." + arguments[0] + ".Plugin", true);
                        if (plugin == null) throw new Exception("Didn't find class " + arguments[1]);
                        plugin.Init(mWorld);
                        mPlugins.Add(plugin);
                        Console.WriteLine("Plugin successfully loaded");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error loading plugin : " + e.Message);
                    }
                }
            }
            #endregion
            #region unknown command
            else
            {
                Console.WriteLine("Unknown command \"" + command + "\"");
            }
            #endregion
        }
        #endregion

        #region Tracking functions
        private void OnHumanDie(object sender, EventArgs e)
        {
            Human who = (Human)sender;
            Console.WriteLine("Human tracker : " + who.InstanceName + " died");
            mTrackedHumans.Remove(who);
        }

        private void OnHumanStartOperation(object sender, EventArgs e)
        {
            Console.WriteLine("Human tracker : " + ((Human)sender).InstanceName + " started doing " + ((OperationEventArgs)e).Operation.ToString());
        }
        #endregion

        #region Main
        static void Main()
        {
            try
            {
                Program program = new Program();
                program.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unhandled exception : " + e.Message);
                Console.Read();
            }
        }
        #endregion
    }
}
