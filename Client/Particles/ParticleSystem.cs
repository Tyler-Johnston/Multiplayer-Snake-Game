// using Microsoft.Xna.Framework;
// using Random;
// using System;
// using System.Collections.Generic;

// namespace CS5410.Particles
// {
//     public class ParticleSystem
//     {
//         private Dictionary<long, Particle> m_particles = new Dictionary<long, Particle>();
//         public Dictionary<long, Particle>.ValueCollection particles { get { return m_particles.Values; } }
//         // private RandomMisc m_random = new RandomMisc();
//         public Vector2 m_center;
//         private int m_sizeMean; // pixels
//         private int m_sizeStdDev;   // pixels
//         private float m_speedMean;  // pixels per millisecond
//         private float m_speedStDev; // pixels per millisecond
//         private float m_lifetimeMean; // milliseconds
//         private float m_lifetimeStdDev; // milliseconds
//         private bool burstComplete = false;
//         private float m_landerRotation;
//         private int m_spread;

//         public ParticleSystem(Vector2 center, int sizeMean, int sizeStdDev, float speedMean, float speedStdDev, int lifetimeMean, int lifetimeStdDev)
//         {
//             m_center = center;
//             m_sizeMean = sizeMean;
//             m_sizeStdDev = sizeStdDev;
//             m_speedMean = speedMean;
//             m_speedStDev = speedStdDev;
//             m_lifetimeMean = lifetimeMean;
//             m_lifetimeStdDev = lifetimeStdDev;
//         }

//         public void ResetBurst()
//         {
//             burstComplete = false;
//         }


//         public void onCatch(int numberOfParticles)
//         {
//             if (!burstComplete)
//             {
//                 for (int i = 0; i < numberOfParticles; i++)
//                 {
//                     Particle newParticle = createCatchParticle();
//                     m_particles.Add(newParticle.name, newParticle);
//                 }
//             }
//             burstComplete = true;
//         }

//         private Particle createCatchParticle()
//         {
//             float size = (float)m_random.nextGaussian(m_sizeMean, m_sizeStdDev);
//             var p = new Particle(
//                     m_center,
//                     m_random.nextCircleVector(),
//                     (float)m_random.nextGaussian(m_speedMean, m_speedStDev),
//                     new Vector2(size, size),
//                     new System.TimeSpan(0, 0, 0, 0, (int)(m_random.nextGaussian(m_lifetimeMean, m_lifetimeStdDev))));
//             return p;
//         }

//         public void update(GameTime gameTime)
//         {
//             List<long> removeMe = new List<long>();
//             foreach (Particle p in m_particles.Values)
//             {
//                 if (!p.update(gameTime))
//                 {
//                     removeMe.Add(p.name);
//                 }
//             }

//             // Remove dead particles
//             foreach (long key in removeMe)
//             {
//                 m_particles.Remove(key);
//             }
//         }
//     }
// }
