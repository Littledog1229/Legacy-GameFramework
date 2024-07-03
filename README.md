# Legacy OpenTK Game Framework

Around some time in 2023 (I can't remember when) I started this project as a for-fun and kind of serious project where I would design my own game framework that I can use for many of the projects I had in my mind at the time.
Work went well, and I think the final thing I had seriously considered implementing was text rendering (and had even started planning), however interest soon dwindled in the project.

This was around the time I started to seriously learn the C++ Programming Language (or maybe it was happening in parallel, I cant remember), and was something that opened up a new world of possibilities and understanding for me. Because of that, though, this project kind of just died.

Working with C++ was new and fun, and this project started to drag (I hate designing UI systems, but it's a necessary evil when it comes to designing a game outside of an engine. Designing UI itself isn't bad though). I simply had other motivations and
had decided to chase them instead of working on a project that got stuck in UI-Framework purgatory. Hell, using some of the libraries I wanted to use was just simpler in C++ since that's what they were originally designed for, so I just focused my interests
there instead of into this project.

Its been around a year since then at the time of writing (I think, its 7/2/2024 at the moment), and I was looking back over some of the projects I could potentially archive and publish for employers and such to view. This was one of the projects that came
to mind, as I ocassionally glaced at to both see how I have progressed and how I used to do things (and even used it as a reference for how to get specific things done that I didn't feel like completely rederiving, I can never remember how to get embedded
resource streams in C#). Its been crazy to look back at this project and see how large it was, and also see how I used to do things (and its also really funny to see ways I haven't changed, like how [Application.cs](https://github.com/Littledog1229/Legacy-GameFramework/blob/master/ApplicationCore/Application/Application.cs) 
looks almost exactly the same as how I still do it in my modern projects).

I have since (maybe like, a week or two ago) started casually working on something similar to this in C# just to both refamiliarize myself with it, but to also create something I can be proud of and reimplement a white-whale project of mine that I have had
for a while now (it's based on an **old Unity prototype** I plan on ripping from my Unity account [using Unity's old VCS stuff]). This new project is using [Veldrid](https://veldrid.dev/) instead of [OpenTK](https://opentk.net/), and personally I think
I am starting to prefer the way it's API is laid out. I've tried it in the past and got stuck since its similar to the API structure Vulkan uses (descriptors man, they were tough for 16 year old me), but now I was finally able to approach and conquor
some of the challenges I had faced (take that, immutable ResourceSets, I found out how to write my Sprite Batch!). Honestly though if I hadn't ever worked with C++ I may have never even branched out to working with thousand-line triangle, and im so glad that
I decided early 2024 to give it a shot ([in a repository available here](https://github.com/Littledog1229/VulkanLearning), though be wary as it may not even work [its also a really big mess]).

Gettng back into the swing with C# has been great, and its been a nice break from C++. Looking back on projects like this has been crazy fun, and I thank whoever is reading this (whether it be an employer, or some random person browning my repositories).



**[TODO: put repository link to that Unity prototype where the words are bolded]**
