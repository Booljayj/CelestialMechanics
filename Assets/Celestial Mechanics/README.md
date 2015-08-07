# Celestial Mechanics Toolkit

This easy-to-use toolkit provides a set of components which can be used to create realistic orbital simulations. GameObjects can move in circular, elliptical, parabolic, or hyperbolic orbits, as well as rotate about an axis. The toolkit also includes components for generating ring meshes, simulating orbital decay and precession, and drawing orbital paths.

Orbital positions are calculated using Kepler's Laws of Planetary Motion. These equations can involve costly calculations, but the Celestial Mechanics Toolkit simplifies these calculations and makes them easy to work into your existing code.

This toolkit is for anyone who wants to create realistic orbital systems, but doesn't want to bother with complicated physics calculations that can easily go out of control. It's also a great way to move any object along an elliptical, parabolic, or hyperbolic path. On top of all this, the toolkit has been designed for quick procedural generation of orbits and orbital systems.

# Orbital Mechanics

## Orbit Shape and Size
There are two basic types of orbits: Closed and Open. Closed orbits repeat, and are used to show planets and satellites. Open orbits do not repeat, and are used to show slingshot or flyby maneuvers. The most common type of orbits are closed elliptical orbits.

The shape of an orbit is defined with the __eccentricity__ parameter. An eccentricity between 0 and 1 defines a closed orbit, and an eccentricity of 1 or greater defines an open orbit. 0 and 1 are special values, and denote perfectly circular and perfectly parabolic orbits, respectively.

The size of an orbit is defined with the __periapsis__ parameter. This indicates the closest distance the orbital body will get to its focus. In an open orbit, the periapsis is usually called the focal length.

## Orbit Orientation
Instead of rotating an orbit using the X, Y, and Z axes, orbital orientations are specified by three other angles: Longitude of the Ascending Node, Inclination, and Argument of the Periapsis.

The __longitude__ is applied first, and indicates the direction of the periapsis within the parent's orbital plane. The __inclination__ is applied second, and indicates the rotation of the orbit about the periapsis. The __argument__ is applied last, and indicates a final rotation of the periapsis within the orbital plane.

## Orbital Motion
The starting position of the orbital body along the orbital path is known as the __mean anomaly__. Anomaly is an angular measurement between the periapsis and the current orbital position.

Open orbits have __limits__, which indicate the lowest and highest anomaly values. Open orbits can theoretically move to infinity, so the limits define a stopping point for the simulation. When an open orbit is simulated, it moves from the mean anomaly to the upper limit.

Closed orbits have an __epoch__ parameter, which is used to adjust the starting time of the simulation. This is useful when simulating real-life orbital systems, which typically record orbital parameters for a single given time. The epoch can adjust this forward or backward to any point in time.

The __period__ is the amount of time it takes for a body to complete one orbit, in seconds. A lower period results in a faster orbital speed. For open orbits, the period is the amount of time to move from the lower limit to the upper limit. The orbital rate can also be adjusted at runtime with the __time scale__ parameter.

## Events
When an orbit starts, stops, and updates, it will invoke UnityEvents. The update event includes a parameter for the current anomaly of the orbital body.

## Properties
The toolkit exposes read-only properties that are used when calculating orbital positions. These properties can be used by custom scripts to perform more detailed calculations.