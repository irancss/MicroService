"use client"
import React from 'react'
import ReactPlayer from 'react-player'

export default function VideoReview({ videoUrl }) {
  return (
    <div className="w-full mx-auto px-4">
      <div className="relative overflow-hidden rounded-lg mb-4">
        <ReactPlayer
          url={videoUrl || 'https://www.youtube.com/watch?v=xvgrtxajeQI&pp=0gcJCYUJAYcqIYzv'}
          className="react-player"
          width="100%"
          height="400px" // ارتفاع ثابت برای جلوگیری از تغییر layout
          controls
          playing={false}
          light={true}
          config={{
            file: {
              attributes: {
                controlsList: 'nodownload',
              },
            },
          }}
          style={{ 
            overflow: 'hidden',
            boxShadow: '0 4px 20px rgba(0, 0, 0, 0.1)',
            backgroundColor: '#000',
            border: '2px solid #fbbf24',
            borderRadius: '20px',
            padding: '10px',
          }}
        /> 
      </div>
    </div>
  );
}