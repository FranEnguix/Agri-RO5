#!/usr/bin/env python3


import rospy
from gazebo_msgs.srv import GetModelState
from geometry_msgs.msg import TransformStamped
import tf2_ros
import tf
import sys
import re

def get_model_state(model_name):
    rospy.wait_for_service('/gazebo/get_model_state')
    try:
        get_model_state_service = rospy.ServiceProxy('/gazebo/get_model_state', GetModelState)
        model_state = get_model_state_service(model_name, 'world')  # 'world' es el nombre del marco de referencia
        return model_state.pose
    except rospy.ServiceException as e:
        rospy.logerr("Error al llamar al servicio get_model_state: %s", e)

def publish_transform(model_name, model_pose,init_x,init_y):
    
    static_transformStamped = TransformStamped()
    static_transformStamped.header.stamp = rospy.Time.now()
    static_transformStamped.header.frame_id = "map"
    static_transformStamped.child_frame_id = model_name+"/odom"

    static_transformStamped.transform.translation.x = init_x
    static_transformStamped.transform.translation.y = init_y
    static_transformStamped.transform.translation.z = 0

    quat = tf.transformations.quaternion_from_euler( 
                   0,0,0)
    static_transformStamped.transform.rotation.x = quat[0]
    static_transformStamped.transform.rotation.y = quat[1]
    static_transformStamped.transform.rotation.z = quat[2]
    static_transformStamped.transform.rotation.w = quat[3]

    tf_broadcaster = tf2_ros.TransformBroadcaster()
    tf_broadcaster.sendTransform(static_transformStamped)
    
    
    
    # Tf ODOM-BASE lINK
    transform_stamped = TransformStamped()
    transform_stamped.header.stamp = rospy.Time.now()
    transform_stamped.header.frame_id = model_name+"/odom"  # Marco de referencia de origen
    transform_stamped.child_frame_id = model_name+"/base_link"  # Marco de referencia del modelo
    transform_stamped.transform.translation.x=model_pose.position.x-init_x
    transform_stamped.transform.translation.y=model_pose.position.y-init_y
    transform_stamped.transform.translation.z=model_pose.position.z

    transform_stamped.transform.rotation = model_pose.orientation
    tf_broadcaster = tf2_ros.TransformBroadcaster()
    tf_broadcaster.sendTransform(transform_stamped)



def main():
    rospy.init_node('precisius_tf', anonymous=True)

    st= sys.argv[5]
    pattern = re.compile(r'robot_(\d+)')
    match = pattern.search(st)
    if match:
        # Obtener el nombre del robot y el número
        robot_name = "robot_" + match.group(1)
    else:
        print("No se encontró el nombre del robot en el string.")

    model_name=robot_name
    model_pose = get_model_state(model_name)
    init_x=model_pose.position.x
    init_y=model_pose.position.y
    rate = rospy.Rate(20)  # Frecuencia de 20 Hz

    while not rospy.is_shutdown():
        model_pose = get_model_state(model_name)
        
        if model_pose:
            publish_transform(model_name, model_pose, init_x, init_y)
        else:
            rospy.logerr(f"No se pudo obtener la posición del modelo {model_name}")

        rate.sleep()


if __name__ == '__main__':
        main()