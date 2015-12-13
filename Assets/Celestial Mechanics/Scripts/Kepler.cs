using UnityEngine;
using System;
using System.Collections;

namespace CelestialMechanics {
	public static class Kepler {
		static int maxIterations = 200;
		static double tolerance = 0.0001;

		#region Computation
		/// <summary>Wraps an angle value between lower and upper.</summary>
		/// <returns>Wrapped angle</returns>
		/// <param name="angle">Angle</param>
		/// <param name="lower">Lower</param>
		/// <param name="upper">Upper</param>
		public static double WrapAngle(double angle, double lower, double upper) {
			if (angle > lower && angle < upper)
				return angle;
			else if (angle < lower)
				return upper - (lower - angle) % (upper - lower);
			else
				return lower + (angle - lower) % (upper - lower);
		}

		/// <summary>Wraps an angle value between lower and upper.</summary>
		/// <returns>Wrapped angle</returns>
		/// <param name="angle">Angle</param>
		/// <param name="lower">Lower</param>
		/// <param name="upper">Upper</param>
		public static float WrapAngle(float angle, float lower, float upper) {
			if (angle > lower && angle < upper)
				return angle;
			else if (angle < lower)
				return upper - (lower - angle) % (upper - lower);
			else
				return lower + (angle - lower) % (upper - lower);
		}

		/// <summary>Compute the rotation defining the orbital plane coordinate system</summary>
		/// <param name="o">longitude of the ascending node [degrees]</param>
		/// <param name="w">argument of the periapsis [degrees]</param>
		/// <param name="i">inclination [degrees]</param>
		/// <returns>Orbital Plane Orientation</returns>
		public static Quaternion ComputeOrientation(float o, float w, float i) {
			return Quaternion.Euler(i, -w, 0f)*Quaternion.Euler(0f, -o, 0f);
		}

		/// <summary>Compute the semi-latus rectum of the conic section</summary>
		/// <param name="a">periapsis [m]</param>
		/// <param name="e">eccentricity [1]</param>
		/// <returns>Semi-Latus Rectum [m]</returns>
		public static double ComputeSemiLatusRectum(double p, double e) {
			if (e == 0.0) {			//circular
				return p;
			} else if (e == 1.0) {	//parabolic
				return p*2.0;
			} else if (e < 1.0) {	//elliptical
				return p*(1.0-e*e)/(1.0-e);
			} else {				//hyperbolic
				return p*(e*e-1.0)/(e-1.0);
			}
		}

		/// <summary>
		/// Computes the semi major axis of the conic section
		/// </summary>
		/// <param name="p">periapsis [m]</param>
		/// <param name="e">eccentricity [1]</param>
		/// <returns>Semi-major axis [m]</returns>
		public static double ComputeSemiMajorAxis(double p, double e) {
			if (e == 0.0) {			//circular
				return p;
			} else if (e == 1.0) {	//parabolic
				return double.PositiveInfinity;
			} else if (e < 1.0) {	//elliptical
				return p/(1-e);
			} else {				//hyperbolic
				return -p/(e-1);
			}
		}

		/// <summary>Compute the rate of orbit</summary>
		/// <param name="T">Period [seconds/orbit]</param>
		/// <param name="from">From Angle [rad]</param>
		/// <param name="to">To Angle [rad]</param>
		/// <returns>Rate [rad/second]</returns>
		public static double ComputeRate(double T, double from, double to) {
			if (T == 0) {
				Debug.LogWarning("Period cannot be 0");
				return 0;
			}
			return Math.Abs((from-to)/T);
		}

		/// <summary>Compute the eccentric anomaly of the conic section</summary>
		/// <param name="M">anomaly [rad]</param>
		/// <param name="e">eccentricity [1]</param>
		/// <returns>Eccentric Anomaly [rad]</returns>
		public static double ComputeEccentricAnomaly(double M, double e) {
			if (e < 0.0)
				throw new ArgumentOutOfRangeException("eccentricity", "cannot be negative");

			if (e == 0.0) {			//circular
				return M;
			} else if (e == 1.0) {	//parabolic
				return NewtonParabolic(M, e);
			} else if (e < 1.0) {	//elliptical
				return NewtonElliptical(M, e);
			} else {				//hyperbolic
				return NewtonHyperbolic(M, e);
			}
		}

		/// <summary>Compute the true anomaly of the conic section.
		/// 	This is the actual angle between the periapsis and the position of
		/// 	the orbital body.
		/// </summary>
		/// <param name="E">eccentric anomaly [rad]</param>
		/// <param name="e">eccentricity [1]</param>
		/// <returns>True Anomaly [rad]</returns>
		public static double ComputeTrueAnomaly(double E, double e) {
			if (e < 0.0)
				throw new ArgumentOutOfRangeException("eccentricity", "cannot be negative");

			if (e == 0.0) {			//circular
				return E;
			} else if (e == 1.0) {	//parabolic
				return 2.0*Math.Atan(E);
			} else if (e < 1.0) {	//elliptical
				return 2.0*Math.Atan2(Math.Sqrt(1.0+e)*Math.Sin(E/2.0), Math.Sqrt(1.0-e)*Math.Cos(E/2.0));
			} else {				//hyperbolic
				return 2.0*Math.Atan2(Math.Sqrt(e+1.0)*Math.Sinh(E/2.0), Math.Sqrt(e-1.0)*Math.Cosh(E/2.0));
			}
		}

		/// <summary>Compute the radius at the given anomaly</summary>
		/// <param name="l">semi latus rectum [m]</param>
		/// <param name="e">eccentricity [1]</param>
		/// <param name="v">true anomaly [rad]</param>
		/// <returns>Radius</returns>
		public static double ComputeRadius(double l, double e, double v) {
			return l/(1.0 + e*Math.Cos(v));
		}

		/// <summary>Compute the position of the body within the orbital plane</summary>
		/// <param name="r">radius [m]</param>
		/// <param name="v">true anomaly [rad]</param>
		/// <returns>Postion Vector</returns>
		public static Vector3 ComputePosition(double r, double v) {
			return new Vector3((float)(r*Math.Cos(v)),
			                   0f,
			                   (float)(r*Math.Sin(v)));
		}

		/// <summary>Compute the velocity of the body within the orbital plane</summary>
		/// <param name="a">semi-major axis [m]</param>
		/// <param name="r">radius [m]</param>
		/// <param name="n">rate [rad/second]</param>
		/// <param name="E">eccentric anomaly [rad]</param>
		/// <param name="e">eccentricity [1]</param>
		/// <returns>Velocity Vector</returns>
		// TODO: support hyperbolic and parabolic orbits
		public static Vector3 ComputeVelocity(double a, double r, double n, double E, double v, double e) {
			if (e == 0.0) {			//circular velocity
				Vector3 vel = new Vector3((float)(-Math.Sin(E)),
				                          0f,
				                          (float)(Math.Cos(E)));
				return (float)(a * n) * vel;

			} else if (e == 1.0) {	//parabolic velocity
				Vector3 vel = new Vector3((float)(-Math.Sin(v)/(Math.Cos(v)+1)),
				                          0f,
				                          1f);
				return (float)Math.Sqrt(n*n*a*a*a*(2/r)) * vel.normalized;

			} else if (e < 1.0) {	//elliptical velocity
				Vector3 vel = new Vector3((float)(-Math.Sin(E)),
			    	                      0f,
			        	                  (float)(Math.Sqrt(1-e*e)*Math.Cos(E)));
				return (float)((a*a*n)/r) * vel;

			} else {				//hyperbolic velocity
				Vector3 vel = new Vector3((float)(-Math.Sinh(E)),
				                          0,
				                          (float)(Math.Sqrt(e*e-1)*Math.Cosh(E)));
				return (float)((a*a*n)/r) * vel;
			}
		}

		/// <summary>Compute the axis a body will rotate around</summary>
		/// <param name="alpha">right ascension [degree]</param>
		/// <param name="delta">declination [degree]</param>
		/// <returns>Quaternion</returns>
		public static Quaternion ComputeAxis(float alpha, float delta) {
			return Quaternion.Euler(delta, alpha, 0f);
		}

		/// <summary>Compute the rotation of a body about its axis</summary>
		/// <param name="axis">axial orientation of the body</param>
		/// <param name="angle">angle of rotation [degree]</param>
		/// <returns>Quaternion</returns>
		public static Quaternion ComputeRotation(Quaternion axis, double angle) {
			return axis * Quaternion.Euler(0f, -(float)angle, 0f);
		}
		#endregion

		#region Newton
		// M = E - e*sin(E)
		// f(E) = E - e*sin(E) - M
		// df/dE = 1 - e*cos(E)
		static double NewtonElliptical(double M, double e) {
			double E0 = M;
			double E = 0;

			for (int i = 0; i < maxIterations; i++) {
				E = E0 - (E0 - e*Math.Sin(E) - M)/(1.0 - e*Math.Cos(E));

				if (Math.Abs(E - e*Math.Sin(E) - M) < tolerance)
					return E;
				else
					E0 = E;
			}
			throw new Exception(string.Format("Max iterations reached without convergence: e={0} M={1}\n" +
				"Consider using eccentricity values that are farther from 1 or anomaly values closer to 0",
				e.ToString(), M.ToString()));
		}

		// M = E + E^3/3
		// f(E) = E + E^3/3 - M
		// df/dE = 1 + E^2
		static double NewtonParabolic(double M, double e) {
			double E0 = M;
			double E = 0;

			for (int i = 0; i < maxIterations; i++) {
				E = E0 - (E0 + E*E*E/3.0 - M)/(1.0 + E0*E0);

				if( Math.Abs(E + E*E*E/3.0 - M) < tolerance)
					return E;
				else
					E0 = E;
			}
			throw new Exception(string.Format("Max iterations reached without convergence: e={0} M={1}\n" +
				"Consider using eccentricity values that are farther from 1 or anomaly values closer to 0",
				e.ToString(), M.ToString()));
		}

		// M = e*sinh(E) - E
		// f(E) = e*sinh(E) - E - M
		// df/dE = e*cosh(E) - 1
		static double NewtonHyperbolic(double M, double e) {
			double E0 = Math.Log(2.0*Math.Abs(M)/e + 1.8);
			double E = 0;

			for (int i = 0; i < maxIterations; i++) {
				E = E0 - (e*Math.Sinh(E0) - E0 - M)/(e*Math.Cosh(E0) - 1.0);

				if (Math.Abs(e*Math.Sinh(E) - E - M) < tolerance)
					return E;
				else
					E0 = E;
			}
			throw new Exception(string.Format("Max iterations reached without convergence: e={0} M={1}\n" +
				"Consider using eccentricity values that are farther from 1 or anomaly values closer to 0",
				e.ToString(), M.ToString()));
		}
		#endregion
	}
}