using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GeneticAlgManager : MonoBehaviour
{
    public GameObject LevelGenerator;

    public int candidateGoal = 10;
    public int offSpringPopulation = 25;
    private float failedYpos;
    public UIManager UImanager;
    public int generation = 1;
    private int candidate = 1;
    private float FitnessTimer;
    private List<List<int[]>> CandidateList = new List<List<int[]>>();
    public List<float> CandidateFitness = new List<float>();
    private List<List<int[]>> CurrentOffspring = new List<List<int[]>>();
    int offspringIter;

    public List<GameObject> levelGMs = new List<GameObject>();

    public List<GameObject> failedJumpPlats = new List<GameObject>();

    int TimeScale = 1;

    List<int[]> finalCandidate = new List<int[]>();

    private int finalGen = 0;

    public Text transitionMatrixVis;

    public SetUpManager setUp;


    // Use this for initialization
    void Start()
    {
        candidateGoal = int.Parse(setUp.candidateReq.text);
        offSpringPopulation = int.Parse(setUp.populationOffspring.text);

        failedYpos = transform.position.y - 1;

        generation = 1;
        candidate = 0;

        UImanager.UpdateCandidate(candidate);
        UImanager.UpdateGeneration(generation);

        levelGMs.Clear();
        int y = 0;
        for (int i = 0; i < setUp.GetTesterNum(); i++)
        {
            y += (int.Parse(setUp.height.text));
            Vector2 pos = transform.position - new Vector3(0, y);
            GameObject gobj = Instantiate(LevelGenerator, pos, transform.rotation);
            levelGMs.Add(gobj);
            y += 10;
        }

        foreach (var LGM in levelGMs)
        {
            LGM.GetComponent<LevelGenerator>().MyStart(int.Parse(setUp.height.text), int.Parse(setUp.length.text));
           LGM.GetComponent<LevelGenerator>().SetRests(setUp.GetRestCov());
            LGM.GetComponent<LevelGenerator>().RandomChain();
            LGM.GetComponent<LevelGenerator>().NewLevelCandidate();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
           IncreaseTimeScale();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DecreaseTimeScale();
        }


        for (int i = 0; i < levelGMs.Count; i++)
        {
            //if fail
            if (levelGMs[i].GetComponent<EventTracker>().FailCheck())
            {
                if (generation == 1)
                {
                    levelGMs[i].GetComponent<LevelGenerator>().RandomChain();
                    levelGMs[i].GetComponent<LevelGenerator>().NewLevelCandidate();
                }
                else
                {
                    offspringIter++;
                    if (offspringIter >= CurrentOffspring.Count)
                        offspringIter = 0;

                    levelGMs[i].GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[offspringIter]);
                    levelGMs[i].GetComponent<LevelGenerator>().NewLevelCandidate();
                }
            }
        }

        bool newGen = false;

        for (int i = 0; i < levelGMs.Count; i++)
        {
            //if success
            if (levelGMs[i].GetComponent<EventTracker>().SuccessCheck())
            {
                if (levelGMs[i].GetComponent<EventTracker>().GetFitness() < 160)
                {
                    AddCandidate(i);
                    FitnessTimer = 0;

                    if (CandidateList.Count >= candidateGoal)
                    {
                        newGen = true;
                    }
                }
                else
                {
                    if (generation == 1)
                    {
                        levelGMs[i].GetComponent<LevelGenerator>().RandomChain();
                        levelGMs[i].GetComponent<LevelGenerator>().NewLevelCandidate();
                    }
                    else
                    {
                        offspringIter++;
                        if (offspringIter >= CurrentOffspring.Count)
                            offspringIter = 0;

                        levelGMs[i].GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[offspringIter]);
                        levelGMs[i].GetComponent<LevelGenerator>().NewLevelCandidate();
                    }
                }
            }
        }

        if (newGen)
        {
            generation++;
            UImanager.UpdateGeneration(generation);
            candidate = 0;
            UImanager.UpdateCandidate(candidate);

            offspringIter = 0;

            CurrentOffspring.Clear();

            for (int i = 0; i < offSpringPopulation; i++)
            {
                Selection();
            }

            CandidateList.Clear();
            CandidateFitness.Clear();

            foreach (var lm in levelGMs)
            {
                lm.GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[offspringIter]);
                lm.GetComponent<LevelGenerator>().NewLevelCandidate();

                offspringIter++;
                if (offspringIter >= CurrentOffspring.Count)
                    offspringIter = 0;
            }

            newGen = false;

        }
    }


    void AddCandidate(int lm)
    {
        candidate++;
        UImanager.UpdateCandidate(candidate);      

        CandidateList.Add(levelGMs[lm].GetComponent<LevelGenerator>().GetGeneratorChromosome());
        CandidateFitness.Add(levelGMs[lm].GetComponent<EventTracker>().GetFitness());

        if (generation == 1)
        {
            levelGMs[lm].GetComponent<LevelGenerator>().RandomChain();
            levelGMs[lm].GetComponent<LevelGenerator>().NewLevelCandidate();
        }
        else
        {
            offspringIter++;
            if (offspringIter >= CurrentOffspring.Count)
                offspringIter = 0;

            levelGMs[lm].GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[offspringIter]);
            levelGMs[lm].GetComponent<LevelGenerator>().NewLevelCandidate();
        }


        CheckForEnd();
    }


    void CheckForEnd()
    {
        foreach (var clm in levelGMs)
        {
            bool fail = false;
            int sucessCount = 0;

            foreach (var lm in levelGMs)
            {
                List<int[]> checkChromo = lm.GetComponent<LevelGenerator>().GetGeneratorChromosome();
                fail = false;

                for (int j = 0; j < checkChromo.Count; j++)
                {
                    for (int i = 0; i < checkChromo[j].Length; i++)
                    {
                        if (checkChromo[j][i] != clm.GetComponent<LevelGenerator>().GetGeneratorChromosome()[j][i])
                        {
                            fail = true;
                        }
                    }
                }

                if (!fail)
                {
                    sucessCount++;
                }
            }

            if (sucessCount >= levelGMs.Count - 1)
            {
                if (generation >= finalGen && finalGen > 0)
                {
                    finalCandidate = clm.GetComponent<LevelGenerator>().GetGeneratorChromosome();

                    transitionMatrixVis.text = "";

                    foreach (var ptl in finalCandidate)
                    {
                        foreach (var i in ptl)
                        {
                            transitionMatrixVis.text += (i.ToString() + ", ");
                        }

                        transitionMatrixVis.text += "\n";
                    }

                    Debug.Log("END FOUND!");
                    UImanager.ShowEndSlate();
                 //   gameObject.SetActive(false);
                    GameObject.Find("SceneManager").GetComponent<SceneManager>().SetChrmo(finalCandidate);
                    GameObject.Find("SceneManager").GetComponent<SceneManager>().SetHeightAndLength((int.Parse(setUp.height.text)), int.Parse(setUp.length.text));

                }
                else
                {

                    Debug.Log("end flagged, addition genertion pass");
                    finalGen = generation + 1;
                }
            }
        }
    }

    void Selection()
    {       
        //uses routlette wheel, TODO - order chain by ascending fitness, use this to determine diffucutly?
        List<int[]> Offspring = new List<int[]>();
    
        int p1 = 0;
        int p2 = 0;

        //float totalFitness = 0;
        //foreach (var f in CandidateFitness)
        //{
        //    totalFitness += f;
        //}

        //float r = Random.Range(0, totalFitness);
        //float iter = 0;
        //int selected = 0;

        //for (int i = 0; i < CandidateFitness.Count; i++)
        //{
        //    if (iter < r)
        //    {
        //        iter += CandidateFitness[i];
        //        p1 = i;
        //    }
        //}

        //r = Random.Range(0, totalFitness);
        //iter = 0;
        //selected = 0;

        //for (int i = 0; i < CandidateFitness.Count; i++)
        //{
        //    if (iter < r)
        //    {
        //        iter += CandidateFitness[i];
        //        p2 = i;
        //    }
        //}

        p1 = Random.Range(0, CandidateList.Count);
        p2 = Random.Range(0, CandidateList.Count);

        while (p2 == p1)
        {
            p2 = Random.Range(0, CandidateList.Count);
        }

        Offspring = Crossover(CandidateList[p1], CandidateList[p2]);

        CurrentOffspring.Add(Offspring);
    }


    List<int[]> Crossover(List<int[]> parent1, List<int[]> parent2)
    {
        List<int[]> Offspring = new List<int[]>();

        bool flipped = false;
        int f = (Random.Range(0, 10));

        if (f >= 5)
        {
            flipped = true;
        }

        int r = Random.Range(0, parent1.Count);

        Offspring.Clear();

        for (int i = 0; i < parent1.Count; i++)
        {
            if (!flipped)
            {
                if (r < i)
                {
                    Offspring.Add(parent1[i]);
                }
                else
                {
                    Offspring.Add(parent2[i]);
                }
            }
            else
            {
                if (r < i)
                {
                    Offspring.Add(parent2[i]);
                }
                else
                {
                    Offspring.Add(parent1[i]);
                }
            }
        }

  //      Debug.Log("Flipped = " + flipped.ToString());
  //      Debug.Log("CrossoverPoint = " + r.ToString());
        return Offspring;
    }


    public void IncreaseTimeScale()
    {
        if (TimeScale < 10)
        {
            TimeScale++;
            Time.timeScale = TimeScale;
            UImanager.UpdateTimeScale(TimeScale);
        }
    }


    public void DecreaseTimeScale()
    {
        if (TimeScale > 1)
        {
            TimeScale--;
            Time.timeScale = TimeScale;
            UImanager.UpdateTimeScale(TimeScale);
        }
    }
}
