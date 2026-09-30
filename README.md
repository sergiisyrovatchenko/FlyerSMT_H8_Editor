# FlyerSMT H8 Editor

Compare, edit and optimize `FlyerSMT` pick-and-place jobs (`.H8`) - quickly and without any pain.

When I started working with `FlyerSMT v2.0`, I ran into annoying UI bugs that sometimes corrupted the job
files for my pick-and-place machines.

With this editor you can open two `H8` files jobs side by side - for example the top and bottom of a board,
or two versions of the same job. The editor highlights every difference, lets you edit and copy values between them,
finds a shorter placement order for the two-nozzle head, and saves HEX safely without touching anything you did not change. 

> Windows desktop app · Windows 7 or later · .NET Framework 4.0 · x86

![FlyerSMT H8 Editor](https://i.postimg.cc/2jc18tZ8/Untitled.png)

## Features

- All `FlyerSMT` settings for `PCB` / `Feeders` / `Components` tabs
- Compare and merge two `.H8` jobs side by side where every difference highlighted
- Changes to a feeder carry over to its items on the `Components` tab automatically
- Ability to disable a specific step on `Components` tab (`-OFF-`)
- Simulate job head path, based on the feeder settings and nozzles configuration
- Optimize finds a shorter placement order
- Import / export components via `CSV` files
- Undo / redo, only the changed bytes are written to `H8` file
- Backup copy is made before the first save

## The `.H8` format

| Offset | Contents |
|---|---|
| 0 | machine profile, 9 ASCII characters + CR LF (`ZB3245TSB`) |
| 11 | u32 — number of components N |
| 15 | N component records of 140 bytes: Designator, Footprint, Value (40 bytes each, Latin-1), X, Y, A (f32), feeder slot (i32), flag (i32) |
| 15 + 140·N | 100 feeder slots of 384 bytes (fields: `H8Feeder.Fields` in [H8File.cs](FlyerSMT_H8_Editor/H8File.cs)) |
| then E | number of boards (i32), 5 × f32 (E+12 = board height), ON[50000] (u8), X[50000], Y[50000], A[50000] (f32) |
